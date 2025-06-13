using RPG_003.Battle.Characters;
using RPG_003.Skills;
using System;

namespace RPG_003.Battle
{
    [Serializable]
    public class DamageInfo
    {
        public ICharacter Source { get; set; }
        public ICharacter Target { get; set; }
        public float Damage { get; set; }
        public Elements Elements { get; set; }
        public AmountAttribute AmountAttribute { get; set; }
        public bool IsCritical { get; set; }
        public DamageInfo(ICharacter source, ICharacter target, float damage, AmountAttribute amount = AmountAttribute.Physic, bool isCritical = false, Elements elements = Elements.None)
        {
            Source = source;
            Target = target;
            Damage = damage;
            IsCritical = isCritical;
            Elements = elements;
        }
    }
}