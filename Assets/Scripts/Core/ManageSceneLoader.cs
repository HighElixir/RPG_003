using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG_003.Core
{
    [DefaultExecutionOrder(-100)]
    public class ManageSceneLoader : MonoBehaviour
    {
        private async void Awake()
        {
            var scene = SceneManager.GetSceneByBuildIndex(5);
            if (!scene.isLoaded)
                await SceneManager.LoadSceneAsync(5, LoadSceneMode.Additive);
        }
    }
}