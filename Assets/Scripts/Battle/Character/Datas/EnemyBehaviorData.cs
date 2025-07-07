using System.Collections.Generic;
using RPG_003.Battle.Behaviour;
using Sirenix.OdinInspector;

namespace RPG_003.Battle
{
    public abstract class EnemyBehaviorData : SerializedScriptableObject
    {
        public abstract List<Skill> Skills { get; }
        public abstract AIBehavior GetCharacterBehaviour();
        public abstract Skill GetSkill(Unit parent);
    }
}
