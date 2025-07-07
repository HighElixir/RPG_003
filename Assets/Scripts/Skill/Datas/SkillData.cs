using RPG_003.Effect;
using System;
using System.Text;
using UnityEngine;

namespace RPG_003.Skills
{
    [Serializable]
    public abstract class SkillData : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private string _name;
        [SerializeField, TextArea(3, 10)] private string _description;
        [SerializeField] private Sprite _defaultIcon;
        [SerializeField] private SoundVFXData _sfx;

        public string ID => _id;
        public string Name => _name;
        public string Description => AdditionalDescriptions(_description);
        public Sprite DefaultIcon => _defaultIcon;
        public SoundVFXData Sfx => _sfx;
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"名前 :" + Name);
            sb.AppendLine($"説明 :" + Description);
            return sb.ToString();
        }

        public virtual string AdditionalDescriptions(string desc)
        {
            return desc;
        }
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(_id))
            {
                var head = GetType().Name switch
                {
                    nameof(BasicData) => "b_",
                    nameof(ModifierData) => "m_",
                    nameof(SmithChip) => "s_",
                    _ => ""
                };
                _id = head + name;
            }
        }
#endif
    }
}