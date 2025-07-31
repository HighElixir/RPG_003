using UniRx;
using UnityEngine;

namespace RPG_003.SceneManage
{
    public class LoadViewBlocker : MonoBehaviour
    {
        [SerializeField] private GameObject _blocker;
        private void Awake()
        {
            SceneLoaderAsync.OnLoadStarted.AsObservable().Subscribe(_ =>
            {
                _blocker.SetActive(true);
            }).AddTo(this);
            SceneLoaderAsync.OnSceneChanged.AsObservable().Subscribe(_ =>
            {
                _blocker.SetActive(false);
            }).AddTo(this);
        }
    }
}