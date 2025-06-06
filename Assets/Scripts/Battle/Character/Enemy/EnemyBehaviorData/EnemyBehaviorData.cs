using RPG_003.Battle.Behaviour;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RPG_003.Battle.Characters.Enemy
{
    public abstract class EnemyBehaviorData : SerializedScriptableObject
    {
        public abstract EnemyBehaviour GetCharacterBehaviour();
        public abstract EnemySkill GetSkill();
    }
}
