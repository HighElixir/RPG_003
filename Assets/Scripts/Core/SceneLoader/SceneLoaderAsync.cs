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

        // シーン遷移許可フラグ
        private bool _allowTransition = false;

        public static UnityAction OnLoadCompleted { get; set; }
        public static UnityAction OnSceneChanged { get; set; }

        // 外部から呼んで「もう遷移OK！」にするメソッド
        public void AllowSceneTransition()
        {
            _allowTransition = true;
        }

        /// <param name="receiver"><see cref="ILoadSceneReceiver">ILoadSceneReceiver</see>の実装が必要</param>
        public void StartSceneLoad(int sceneId, GameObject receiver = null, bool changedirect = true)
        {
            _from = SceneManager.GetActiveScene();
            _allowTransition = changedirect;
            LoadSceneAsync(sceneId, receiver).Forget();
        }

        private async UniTaskVoid LoadSceneAsync(int sceneId, GameObject receiver)
        {
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
            await SceneManager.UnloadSceneAsync(_from).ToUniTask();
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
