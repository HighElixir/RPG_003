﻿using HighElixir.Utilities;
using RPG_003.Battle;
using System;
using System.Collections.Generic;
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
        private BattleManager _battleManager;
        private int _from;

        // === Public ===
        public void ToBattleScene(SceneLoaderAsync loader)
        {
            _from = SceneManager.GetActiveScene().buildIndex;
            SceneLoaderAsync.OnLoadCompleted += OnSceneLoaded;
            loader.StartSceneLoad(_battleSceneId);
        }
        public void BackScene(SceneLoaderAsync loader)
        {
            loader.StartSceneLoad(_from);
        }
        public void OnSceneLoaded()
        {
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

        public void StartBattle(int wave, List<Player> players)
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

        public void SetBattleManageer(BattleManager battleManager)
        {
            _battleManager = battleManager;
        }
        // === Private ===
        // === Unity ===
        protected override void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}