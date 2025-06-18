using RPG_003.Battle.Factions;
using RPG_003.Status;
using RPG_003.Skills;
using Sirenix.OdinInspector;
using UnityEngine;
using System.Collections.Generic;

namespace RPG_003.Battle.Characters.Enemy
{
    [CreateAssetMenu(fileName = "EnemySkill", menuName = "RPG_003/Enemy/EnemySkill", order = 1)]
    public class EnemySkill : SerializedScriptableObject
    {
        public string skillName;
        public Elements elements;
        public AmountAttribute skillType;
        public List<DamageData> damageDatas;
        public TargetData targetData;
        public bool canSecanSelectSameTargetlect = true;
        // Note : 今後追加デバフなどを追加する

        public Faction GetTargetFaction()
        {
            return targetData.Faction switch
            {
                Faction.Enemy => Faction.Ally,
                Faction.Ally => Faction.Enemy,
                _ => targetData.Faction,
            };
        }
    }
}
// unicode