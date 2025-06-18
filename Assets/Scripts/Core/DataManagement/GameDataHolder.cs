using HighElixir.Utilities;
using UnityEngine;
using System.Collections.Generic;
using RPG_003.Battle.Characters.Player;
using RPG_003.Skills;

namespace RPG_003.Core
{
    /// <summary>
    /// セーブデータからの読み込み、一時データの保持、保存を担う
    /// </summary>
    public class GameDataHolder : SingletonBehavior<GameDataHolder>
    {
        // == holders ==
        [SerializeField] private List<PlayerDataHolder> _players = new List<PlayerDataHolder>();

        // 全てのスキルへの参照を持つ
        [SerializeField] private List<SkillDataHolder> skillDatas = new List<SkillDataHolder>();

        // == property ==
        public List<PlayerDataHolder> Players => _players;

        // === Public ===
        // PlayerDataHolder関連
        public void AddPlayerData(PlayerDataHolder player) => _players.Add(player);
        public void RemovePlayerData(PlayerDataHolder remove) => _players.Remove(remove);
        public void SetPlayerDatas(List<PlayerDataHolder> data) => _players = new(data);
        public List<PlayerData> GetPlayerDatas()
        {
            return _players.ConvertAll<PlayerData>((item) => item.Convert());
        }
        // === UnityLifecycle ===
        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }
    }
}