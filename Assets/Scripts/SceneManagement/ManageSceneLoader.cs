using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG_003.SceneManage
{
    [DefaultExecutionOrder(-100)]
    public class ManageSceneLoader : MonoBehaviour
    {
        private static readonly string _manager = "ManagerScene";
        private async void Awake()
        {
            var scene = SceneManager.GetSceneByName(_manager);
            if (!scene.isLoaded)
            {
                await SceneManager.LoadSceneAsync(_manager, LoadSceneMode.Additive);
                scene = SceneManager.GetSceneByName(_manager);
                SceneLoaderAsync.managerScene = scene;
            }
        }
    }
}