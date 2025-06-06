namespace RPG_003.Battle.Skills
{
    using RPG_003.Battle.Characters;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// スキルの細かい実装が決まっていないため簡素な実装
    /// </summary>
    public class Skill
    {
        public DamageInfo damageInfo;
        public string skillName;
        public void Execute(List<CharacterBase> targets)
        {
            foreach (var target in targets)
            {
                Debug.Log($"Executing skill on {target.Data.Name}");
                target.TakeDamage(damageInfo);
            }
        }
    }
}