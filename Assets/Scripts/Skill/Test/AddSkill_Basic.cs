using RPG_003.Effect;
using Sirenix.OdinInspector;
using UnityEngine;
namespace RPG_003.Skills
{
    public class AddSkill_Basic : MonoBehaviour
    {
        [SerializeField] private BattleExecute _execute;
        [SerializeField] private BasicData _basicData;
        [SerializeField] private string _skillName;
        [SerializeField] private Sprite _icon;
        [SerializeField] private SoundVFXData _soundVFXData;

        [Button("AddSkill")]
        private void AddSkill()
        {
            _execute.AddSkill(Inner());
        }
        protected BasicHolder Inner()
        {
            var h = new BasicHolder();
            h.SetSkillData(_basicData);
            h.SetCustomName(_skillName);
            h.SetIcon(_icon);
            h.SetSoundVFXData(_soundVFXData);
            return h;
        }
    }
}