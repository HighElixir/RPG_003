using RPG_003.Status;
using RPG_003.Core;
using Sirenix.OdinInspector;
using UnityEngine;
using RPG_003.Character;

namespace RPG_003.Skills
{
    public class BattleExecute : MonoBehaviour
    {
        [SerializeField] private PlayerDataHolder _players = new();
        [SerializeField] private SceneLoaderAsync _sceneLoader;
        [SerializeField] protected LoadingUIController _loadingUIController;
        [SerializeField, BoxGroup("CharacterEditor")] private StatusData _addData;
        [SerializeField, BoxGroup("CharacterEditor/Skill"), PropertyRange(0, 4)] private int _skillAddTarget;
        [SerializeField, BoxGroup("CharacterEditor/Skill")] private string _name;
        // === Public ===
        public void AddSkill(SkillHolder skill)
        {
            if (_players.Data[_skillAddTarget].Skills.Count < 3)
            {
                _players.Data[_skillAddTarget].Skills.Add(skill);
                Debug.Log($"Success add skill({skill.Name}) for {_players.Data[_skillAddTarget].Name}");
                if (skill.Icon == null)
                    Debug.Log("Icon が null!");
            }
            else
            {
                Debug.LogError($"{_players.Data[_skillAddTarget].Name}のSkillが3つを超えてるよ！");
            }
        }

        // === Private ===
        [Button("CreateCharacter")]
        private void CreateCharacter()
        {
            if (_players.Count < 4)
                _players.Add(new CharacterDataHolder().SetStatus(_addData));
            else
                Debug.LogError("4人まで！");
        }
        [Button("Set Default")]
        private void SetData()
        {
            _addData = new StatusData()
                .SetHp(CoreDatas.HP)
                .SetMp(CoreDatas.MP)
                .SetStr(CoreDatas.STR)
                .SetInt(CoreDatas.INT)
                .SetDef(CoreDatas.DEF)
                .SetMDef(CoreDatas.MDEF)
                .SetLuk(CoreDatas.LUK)
                .SetCriticalRate(CoreDatas.CRR)
                .SetCriticalDamage(CoreDatas.CRD)
                .SetTakeDamageScale(1f)
                .ListInit();
        }
        [Button("StartBattle")]
        public void BattleStart()
        {
            if (UnityEditor.EditorApplication.isPlaying)
            {
                foreach (var player in _players.Data)
                {
                    GameDataHolder.instance.Players.Add(player);
                }
                BattleSceneManager.instance.ToBattleScene(_sceneLoader, _loadingUIController.gameObject);
            }
        }
        // === Unity ===
        private void OnValidate()
        {
            _skillAddTarget = Mathf.Clamp(_skillAddTarget, 0, _players.Data.Count - 1);
            if (_players.Data.Count == 0) return;
            _name = _players.Data[_skillAddTarget].ConvertedData.Name;
        }
    }
}