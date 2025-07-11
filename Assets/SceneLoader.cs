using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector.Editor.Drawers;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG_003.Core
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private LoadingUIController _uicontroller;
        private static SceneLoaderAsync _sceneLoaderAsync;
        public static SceneLoaderAsync SceneLoaderAsync => _sceneLoaderAsync;
        public void SceneLoad(int sceneId)
        {
            _sceneLoaderAsync.StartSceneLoad(sceneId, _sceneLoaderAsync.gameObject);
        }
        public void SceneToBefore()
        {
            _sceneLoaderAsync.SceneChangeBefore(_sceneLoaderAsync.gameObject);
        }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private async void Start()
        {
            if (_sceneLoaderAsync != null) return;
            await UniTask.WaitUntil(() => SceneManager.GetSceneByName("ManagerScene").isLoaded);
            if (_sceneLoaderAsync != null) return;
            foreach (var item in SceneManager.GetSceneByName("ManagerScene").GetRootGameObjects())
            {
                if (item.TryGetComponent<SceneLoaderAsync>(out var loader))
                {
                    _sceneLoaderAsync = loader;
                    return;
                }
            }
        }
    }
}