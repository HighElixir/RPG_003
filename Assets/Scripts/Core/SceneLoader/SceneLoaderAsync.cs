using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace RPG_003.Core
{
    // SceneLoaderAsync.cs
    public class SceneLoaderAsync : MonoBehaviour
    {
        [SerializeField] private LoadingUIController loadingUI;

        public static UnityAction OnLoadCompleted { get; set; }

        public void StartSceneLoad(int sceneId)
        {
            LoadSceneAsync(sceneId).Forget();
        }

        private async UniTaskVoid LoadSceneAsync(int sceneId)
        {
            loadingUI.OnStart();

            var operation = SceneManager.LoadSceneAsync(sceneId);
            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / 0.9f);
                loadingUI.UpdateProgress(progress);
                await UniTask.Yield();
            }

            loadingUI.OnComplete();
            OnLoadCompleted?.Invoke();
        }
    }
}