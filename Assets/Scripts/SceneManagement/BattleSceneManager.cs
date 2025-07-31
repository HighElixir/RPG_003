using HighElixir;
using RPG_003.Battle;
using RPG_003.SceneManage;
using System;
using UnityEngine;

namespace RPG_003.Core
{
    public class BattleSceneExecuter : SingletonBehavior<BattleSceneExecuter>, IOnSceneChanged
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
        private int _tutor;
        // === Public ===
        public void ToBattleScene(int tutor = -1)
        {
            _tutor = tutor;
            _sceneLoader.StartSceneLoad(_battleSceneId, gameObject);
        }
        public void BackScene()
        {
            _sceneLoader.SceneChangeBefore();
        }

        //public async UniTask StartBattle(int wave, List<Unit> players)
        //{
        //    UniTask task;
        //    switch (wave)
        //    {
        //        case 1:
        //            task = _battleManager.Setup(players, _battleData.Wave1);
        //            break;
        //        case 2:
        //            task = _battleManager.Setup(players, _battleData.Wave2);
        //            break;
        //        case 3:
        //            task = _battleManager.Setup(players, _battleData.Wave3);
        //            break;
        //        default:
        //            Debug.LogError("Invalid wave number.");
        //            return;
        //    }
        //    await task;
        //    _battleManager.StartBattle();
        //}

        public async void OnSceneChanged()
        {
            if (_battleManager == null) _battleManager = BattleManager.instance ?? null;
            if (_battleManager == null)
            {
                Debug.LogError("BattleManager not found in the scene.");
                return;
            }
            await _battleManager.Setup(GameDataHolder.instance.GetPlayerDatas(), _battleData.Wave1);

            _battleManager.StartBattle();
        }
    }
}