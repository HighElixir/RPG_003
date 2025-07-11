using HighElixir;
using RPG_003.Battle;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG_003.Core
{
    public class BattleSceneManager : SingletonBehavior<BattleSceneManager>
    {
        [Serializable]
        private class BattleData
        {
            public SpawningTable Wave1;
            public SpawningTable Wave2;
            public SpawningTable Wave3;
        }
        [SerializeField] private int _battleSceneId;
        [SerializeField] private BattleData _battleData;
        [SerializeField] private SceneLoaderAsync _sceneLoader;
        private BattleManager _battleManager;
        private IDisposable _loader;
        private int _tutor;
        // === Public ===
        public void ToBattleScene(GameObject receiver = null, int tutor = -1)
        {
            _tutor = tutor;
            _loader = SceneLoaderAsync.OnSceneChanged.AsObservable().Subscribe(_ => OnSceneChanged()).AddTo(this);
            _sceneLoader.StartSceneLoad(_battleSceneId, receiver);
        }
        public void BackScene()
        {
            _sceneLoader.SceneChangeBefore();
        }
        public void OnSceneChanged()
        {
            _loader.Dispose();
            if (_battleManager == null)
            {
                foreach (var obj in SceneManager.GetActiveScene().GetRootGameObjects())
                {
                    if (obj.TryGetComponent<BattleManager>(out var m))
                    {
                        _battleManager = m;
                    }
                }
            }
            Debug.Log("Start");
            _battleManager.StartBattle(GameDataHolder.instance.GetPlayerDatas(), _battleData.Wave1);
        }

        public void StartBattle(int wave, List<RPG_003.Battle.Unit> players)
        {
            switch (wave)
            {
                case 1:
                    _battleManager.StartBattle(players, _battleData.Wave1);
                    break;
                case 2:
                    _battleManager.StartBattle(players, _battleData.Wave2);
                    break;
                case 3:
                    _battleManager.StartBattle(players, _battleData.Wave3);
                    break;
                default:
                    Debug.LogError("Invalid wave number.");
                    break;
            }
        }

        public void SetBattleManager(BattleManager battleManager)
        {
            _battleManager = battleManager;
        }
    }
}