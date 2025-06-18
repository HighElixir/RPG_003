using RPG_003.Effect;
using UnityEngine;
namespace RPG_003.Skills
{
    public class AddSkill_Basic : AddSkills
    {
        [SerializeField] private BasicData _basicData;
        [SerializeField] private string _skillName;
        [SerializeField] private Sprite _icon;
        [SerializeField] private SoundVFXData _soundVFXData;
        protected override SkillDataHolder Inner()
        {
            var h = new BasicHolder(_basicData);
            h.SetCustomName(_skillName);
            h.SetIcon(_icon);
            h.SetSoundVFXData(_soundVFXData);
            return h;
        }
    }
}