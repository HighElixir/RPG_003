using RPG_003.Core;
using UnityEngine;

namespace RPG_003.Title
{
#if UNITY_EDITOR
    public class ToEdit : MonoBehaviour
    {
        [SerializeField] LoadingUIController _ui;
        [SerializeField] SceneLoaderAsync _loader;
        public void ToEditScene()
        {
            _loader.StartSceneLoad(6, _ui.gameObject);
        }
    }
#endif
}