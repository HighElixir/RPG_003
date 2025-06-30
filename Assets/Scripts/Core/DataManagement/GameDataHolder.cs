using HighElixir.Utilities;
using RPG_003.Battle;
using RPG_003.Skills;
using System.Collections.Generic;
using UnityEngine;

namespace RPG_003.Core
{
    /// <summary>
    /// セーブデータからの読み込み、一時データの保持、保存を担う
    /// </summary>
    public class GameDataHolder : SingletonBehavior<GameDataHolder>
    {
        // == holders ==
        [SerializeField] private List<PlayerDataHolder> _players = new List<PlayerDataHolder>();
        [SerializeField] private List<SkillDataHolder> _skillDatas = new List<SkillDataHolder>();// 全てのスキルへの参照を持つ
        [SerializeField] private ItemHolder _itemHolder = new();
        [SerializeField] private SkillDatasHolder _skillDataCount = new();

        // == property ==
        public IReadOnlyList<PlayerDataHolder> Players => _players.AsReadOnly();
        public IReadOnlyList<SkillDataHolder> Skills => _skillDatas.AsReadOnly();
        public ItemHolder Items => _itemHolder;
        public SkillDatasHolder SkillDatas => _skillDataCount;
        // === Public ===
        // PlayerDataHolder関連
        public void AddPlayerData(PlayerDataHolder player) => _players.Add(player);
        public void RemovePlayerData(PlayerDataHolder remove) => _players.Remove(remove);
        public void SetPlayerDatas(List<PlayerDataHolder> data) => _players = new(data);
        public List<PlayerData> GetPlayerDatas()
        {
            return _players.ConvertAll<PlayerData>(item => item.Convert());
        }
        // SkillHolder
        public void AddSkill(SkillDataHolder skill) => _skillDatas.Add(skill);
        public void RemoveSkill(SkillDataHolder skill) => _skillDatas.Remove(skill);

        // === UnityLifecycle ===
        protected override void Awake()
        {
            base.Awake();
            if (gameObject)
                DontDestroyOnLoad(gameObject);
        }
    }
}