using System.Text;
using UnityEngine;

namespace RPG_003.DataManagements.Datas
{
    public abstract class SmithChip : SkillData
    {
        [SerializeField] protected float _load;
        [SerializeField] protected float _power; // consume or production

        public float Load => _load;
        public float Power => _power;

        public override string AdditionalDescriptions(string desc)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(desc);
            if (_load > 0) sb.AppendLine($"[負荷] :{_load}");
            if (_power != 0) sb.AppendLine($"[{(_power > 0 ? "生産動力" : "消費動力")}] :{Mathf.Abs(_power)}");
            return sb.ToString();
        }
    }
}