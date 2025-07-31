using RPG_003.DataManagements.Datas;
using System;

namespace RPG_003.Battle
{
    [Serializable]
    public class DamageInfo : ICloneable
    {
        public Unit Source { get; set; }
        public Unit Target { get; set; }
        public Skill Skill { get; set; }
        public float Damage { get; set; }
        public Elements Elements { get; set; }
        public AmountAttribute AmountAttribute { get; set; }
        public bool IsCritical { get; set; }
        public DamageInfo(Unit source, Unit target, float damage, AmountAttribute amount = AmountAttribute.Physic, bool isCritical = false, Elements elements = Elements.None)
        {
            Source = source;
            Target = target;
            Damage = damage;
            IsCritical = isCritical;
            Elements = elements;
        }
        public DamageInfo() { }
        public object Clone()
        {
            var clone = new DamageInfo(Source, Target, Damage, AmountAttribute, IsCritical, Elements)
            {
                Source = Source,
                Target = Target,
                Damage = Damage,
                AmountAttribute = AmountAttribute,
                IsCritical = IsCritical,
                Elements = Elements
            };
            return clone;
        }
        public override string ToString()
        {
            return $"DamageInfo: Source={Source?.Data.Name}, Target={Target?.Data.Name}, Damage={Damage}, Elements={Elements}, AmountAttribute={AmountAttribute}, IsCritical={IsCritical}";
        }
    }
}