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
        [SerializeField] private PlayerDataHolder _players = new();
        [SerializeField] private List<SkillHolder> _skillDatas = new();// 全てのスキルへの参照を持つ
        [SerializeField] private ItemHolder _itemHolder = new();
        [SerializeField] private SkillDataHolder _skillDataCount = new();

        // == property ==
        public PlayerDataHolder Players => _players;
        public IReadOnlyList<SkillHolder> Skills => _skillDatas.AsReadOnly();
        public ItemHolder Items => _itemHolder;
        public SkillDataHolder SkillDatas => _skillDataCount;
        // === Public ===
        // PlayerDataHolder関連
        public List<PlayerData> GetPlayerDatas()
        {
            return _players.Data.ConvertAll<PlayerData>(item => item.Convert());
        }
        // SkillHolder
        public void AddSkill(SkillHolder skill) => _skillDatas.Add(skill);
        public void RemoveSkill(SkillHolder skill) => _skillDatas.Remove(skill);

        // === UnityLifecycle ===
        protected override void Awake()
        {
            base.Awake();
        }
    }
}