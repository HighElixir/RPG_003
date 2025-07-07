using RPG_003.Status;
using System.Text;

namespace RPG_003.Character
{
    public class CharacterStatsBuilder
    {
        private StringBuilder _sb = new StringBuilder();
        private CharacterDataHolder _character;
        public CharacterStatsBuilder Enter(CharacterDataHolder character)
        {
            _sb.Clear();
            _character = character;
            return this;
        }
        public CharacterStatsBuilder AddName()
        {
            _sb.AppendLine("名前: " + _character.Name);
            return this;
        }
        public CharacterStatsBuilder AddStatus()
        {
            var s = _character.ConvertedData;
            _sb.AppendLine("最大HP: " + s.HP);
            _sb.AppendLine("最大MP: " + s.MP);
            _sb.AppendLine("攻撃力: " + s.STR);
            _sb.AppendLine("魔力: " + s.INT);
            _sb.AppendLine("防御力: " + s.DEF);
            _sb.AppendLine("魔法防御力: " + s.MDEF);
            _sb.AppendLine("スピード: " + s.SPD);
            _sb.AppendLine("幸運: " + s.LUK);
            _sb.AppendLine("受けるダメージ倍率: " + s.TakeDamageScale * 100 + "%");
            _sb.AppendLine("会心率: " + s.CR * 100 + "%");
            _sb.AppendLine("会心ダメージ: " + s.CRDamage * 100 + "%");
            return this;
        }
        public CharacterStatsBuilder AddElementData()
        {
            var take = _character.ConvertedData.TakeElementDamageScale;
            var give = _character.ConvertedData.GiveElementDamageScale;
            for (var i = 0; i < take.Count; i++)
            {
                _sb.AppendLine($"受ける{take[i].element.ToJapanese()}ダメージ: {take[i].scale * 100}%");
                _sb.AppendLine($"与える{give[i].element.ToJapanese()}ダメージ: {give[i].scale * 100}%");
            }
            return this;
        }
        public string Build()
        {
            return _sb.ToString();
        }
    }
}