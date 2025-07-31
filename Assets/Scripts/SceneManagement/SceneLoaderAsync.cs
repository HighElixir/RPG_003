using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace RPG_003.SceneManage
{
    public class SceneLoaderAsync : MonoBehaviour
    {
        private Scene _from;
        private static int _beforeSceneId;
        private static string _beforeSceneAddress;
        public static Scene managerScene;

        private bool _allowTransition = false;

        public static UnityEvent OnLoadStarted { get; set; } = new UnityEvent();
        public static UnityEvent OnLoadCompleted { get; set; } = new UnityEvent();
        public static UnityEvent OnSceneChanged { get; set; } = new UnityEvent();

        public void AllowSceneTransition() => _allowTransition = true;

        // --------------------
        // ① BuildIndexで遷移
        // --------------------
        public void StartSceneLoad(int sceneId, GameObject receiver = null, bool changeDirectry = true)
        {
            _from = SceneManager.GetActiveScene();
            _beforeSceneId = _from.buildIndex;
            _beforeSceneAddress = null;
            _allowTransition = changeDirectry;

            LoadSceneByIndexAsync(sceneId, receiver).Forget();
        }

        // --------------------
        // ② Addressablesで遷移
        // --------------------
        public void StartSceneLoad(string sceneAddress, GameObject receiver = null, bool changeDirectry = true)
        {
            _from = SceneManager.GetActiveScene();
            _beforeSceneAddress = sceneAddress;
            _beforeSceneId = -1;
            _allowTransition = changeDirectry;

            LoadSceneByAddressAsync(sceneAddress, receiver).Forget();
        }

        // --------------------
        // ③ 戻る処理（どっちでもOK）
        // --------------------
        public void SceneChangeBefore(GameObject receiver = null, bool changeDirectry = true)
        {
            if (_from == null) return;

            if (!string.IsNullOrEmpty(_beforeSceneAddress))
            {
                StartSceneLoad(_beforeSceneAddress, receiver, changeDirectry);
            }
            else
            {
                StartSceneLoad(_beforeSceneId, receiver, changeDirectry);
            }
        }

        // --------------------
        // BuildIndexシーンのロード処理
        // --------------------
        private async UniTaskVoid LoadSceneByIndexAsync(int sceneId, GameObject receiver)
        {
            SceneManager.SetActiveScene(managerScene);
            await SceneManager.UnloadSceneAsync(_from).ToUniTask();

            var operation = SceneManager.LoadSceneAsync(sceneId, LoadSceneMode.Additive);
            operation.allowSceneActivation = false;

            HandleLoadingProgress(operation, receiver);

            await UniTask.WaitUntil(() => _allowTransition);
            operation.allowSceneActivation = true;
            await operation.ToUniTask();

            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(sceneId));
            FinalizeSceneChange(receiver);
        }

        // --------------------
        // Addressablesシーンのロード処理
        // --------------------
        private async UniTaskVoid LoadSceneByAddressAsync(string sceneAddress, GameObject receiver)
        {
            SceneManager.SetActiveScene(managerScene);
            await SceneManager.UnloadSceneAsync(_from).ToUniTask();

            var handle = Addressables.LoadSceneAsync(sceneAddress, LoadSceneMode.Additive, false);
            HandleLoadingProgress(handle, receiver);

            await UniTask.WaitUntil(() => _allowTransition);
            await handle.Result.ActivateAsync().ToUniTask();

            SceneManager.SetActiveScene(handle.Result.Scene);
            FinalizeSceneChange(receiver);
        }

        // --------------------
        // 共通：進捗処理
        // --------------------
        private async void HandleLoadingProgress(AsyncOperation operation, GameObject receiver)
        {
            OnLoadStarted?.Invoke();
            ExecuteEvents.Execute<IOnLoadStart>(receiver, null, (r, _) => r.OnStart());

            while (operation.progress < 0.9f)
            {
                float progress = Mathf.Clamp01(operation.progress / 0.9f);
                ExecuteEvents.Execute<IOnLoadProcess>(receiver, null, (r, _) => r.OnProcess(progress));
                await UniTask.Yield();
            }

            OnLoadCompleted?.Invoke();
            ExecuteEvents.Execute<IOnLoadCompleted>(receiver, null, (r, _) => r.OnLoadCompleted());
        }

        private async void HandleLoadingProgress(AsyncOperationHandle<SceneInstance> handle, GameObject receiver)
        {
            OnLoadStarted?.Invoke();
            ExecuteEvents.Execute<IOnLoadStart>(receiver, null, (r, _) => r.OnStart());

            while (!handle.IsDone && handle.PercentComplete < 0.9f)
            {
                float progress = Mathf.Clamp01(handle.PercentComplete / 0.9f);
                ExecuteEvents.Execute<IOnLoadProcess>(receiver, null, (r, _) => r.OnProcess(progress));
                await UniTask.Yield();
            }

            OnLoadCompleted?.Invoke();
            ExecuteEvents.Execute<IOnLoadCompleted>(receiver, null, (r, _) => r.OnLoadCompleted());
        }

        // --------------------
        // 共通：終了処理
        // --------------------
        private void FinalizeSceneChange(GameObject receiver)
        {
            _ = Resources.UnloadUnusedAssets();

            ExecuteEvents.Execute<IOnSceneChanged>(receiver, null, (r, _) => r.OnSceneChanged());
            OnSceneChanged?.Invoke();
        }
    }

    public interface IOnLoadStart : IEventSystemHandler { void OnStart(); }
    public interface IOnLoadCompleted : IEventSystemHandler { void OnLoadCompleted(); }
    public interface IOnSceneChanged : IEventSystemHandler { void OnSceneChanged(); }
    public interface IOnLoadProcess : IEventSystemHandler { void OnProcess(float progress); }
}
