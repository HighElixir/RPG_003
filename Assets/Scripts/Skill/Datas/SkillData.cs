using System;
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

        public string ID => _id;
        public string Name => _name;
        public string Description => _description;
        public Sprite DefaultIcon => _defaultIcon;
        public override string ToString() => $"{ID}:{Name}";

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(_id))
                Debug.LogWarning($"[{name}] IDが空だよ！id決めてね");
        }

        private void Reset()
        {
            var head = GetType().ToString() switch
            {
                nameof(BasicData) => "b_",
                nameof(ModifierData) => "m_",
                _ => ""
            };
            _id = head + name;
        }
#endif

    }
}