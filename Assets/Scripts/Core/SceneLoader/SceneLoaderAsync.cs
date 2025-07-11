using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace RPG_003.Core
{
    public class SceneLoaderAsync : MonoBehaviour
    {
        private Scene _from;
        private static int _beforeSceneId;
        public static Scene managerScene;

        // シーン遷移許可フラグ
        private bool _allowTransition = false;

        public static UnityEvent OnLoadStarted { get; set; } = new UnityEvent();
        public static UnityEvent OnLoadCompleted { get; set; } = new UnityEvent();
        public static UnityEvent OnSceneChanged { get; set; } = new UnityEvent();

        // 外部から呼んで「もう遷移OK！」にするメソッド
        public void AllowSceneTransition()
        {
            _allowTransition = true;
        }

        /// <param name="receiver"><see cref="ILoadSceneReceiver">ILoadSceneReceiver</see>の実装が必要</param>
        public void StartSceneLoad(int sceneId, GameObject receiver = null, bool changeDirectry = true)
        {
            _from = SceneManager.GetActiveScene();
            _beforeSceneId = _from.buildIndex;
            _allowTransition = changeDirectry;
            LoadSceneAsync(sceneId, receiver).Forget();
        }
        public void SceneChangeBefore(GameObject reciver = null, bool changeDirectry = true)
        {
            if (_from == null) return;
            StartSceneLoad(_beforeSceneId, reciver, changeDirectry);
        }
        private async UniTaskVoid LoadSceneAsync(int sceneId, GameObject receiver)
        {
            SceneManager.SetActiveScene(managerScene);
            await SceneManager.UnloadSceneAsync(_from).ToUniTask();
            var operation = SceneManager.LoadSceneAsync(sceneId, LoadSceneMode.Additive);
            operation.allowSceneActivation = false;
            if (receiver != null)
                ExecuteEvents.Execute<ILoadSceneReceiver>(receiver, null, (reciever, _) => reciever.OnStart());
            while (operation.progress < 0.9f)
            {
                if (receiver != null)
                {
                    float progress = Mathf.Clamp01(operation.progress / 0.9f);
                    ExecuteEvents.Execute<ILoadSceneReceiver>(receiver, null, (reciever, _) => reciever.OnProcess(progress));
                }
                await UniTask.Yield();
            }

            // ロード完了演出
            OnLoadCompleted?.Invoke();
            // ここで許可が出るまで待機
            await UniTask.WaitUntil(() => _allowTransition);
            operation.allowSceneActivation = true;
            await operation.ToUniTask();
            Debug.Log("Scene");
            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(sceneId));
            _ = Resources.UnloadUnusedAssets();
            if (receiver != null)
                ExecuteEvents.Execute<ILoadSceneReceiver>(receiver, null, (reciever, _) => reciever.OnCompleted());
            OnSceneChanged?.Invoke();
        }
    }

    public interface ILoadSceneReceiver : IEventSystemHandler
    {
        void OnStart();
        void OnCompleted();
        void OnProcess(float progress);
    }
}
