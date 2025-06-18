using RPG_003.Battle.Characters;
using RPG_003.Core;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace RPG_003.Skills
{
    public class BattleExecute : MonoBehaviour
    {
        [SerializeField] private SceneLoaderAsync _sceneLoader;
        [SerializeField] private List<PlayerDataHolder> _players = new();
        [SerializeField, BoxGroup("CharacterEditor")] private CharacterData _addData;
        [SerializeField, BoxGroup("CharacterEditor/Skill"), PropertyRange(0, 4)] private int _skillAddTarget;
        [SerializeField, BoxGroup("CharacterEditor/Skill")] private string _name;
        // === Public ===
        public void AddSkill(SkillDataHolder skill)
        {
            if (_players[_skillAddTarget].Skills.Count < 3)
            {
                _players[_skillAddTarget].Skills.Add(skill);
                Debug.Log($"Success add skill({skill.Name}) for {_players[_skillAddTarget].CharacterData.Name}");
            }
            else
            {
                Debug.LogError($"{_players[_skillAddTarget].CharacterData.Name}のSkillが3つを超えてるよ！");
            }
        }

        // === Private ===
        [Button("CreateCharacter")]
        private void CreateCharacter()
        {
            if (_players.Count < 4)
                _players.Add(new PlayerDataHolder(_addData));
            else
                Debug.LogError("4人まで！");

        }
        [Button("StartBattle")]
        private void BattleStart()
        {
            GameDataHolder.instance.SetPlayerDatas(_players);
            BattleSceneManager.instance.ToBattleScene(_sceneLoader);
        }
        // === Unity ===
        private void OnValidate()
        {
            _skillAddTarget = Mathf.Clamp(_skillAddTarget, 0, _players.Count - 1);
            if (_players.Count == 0) return;
            _name = _players[_skillAddTarget].CharacterData.Name;
        }
    }
}