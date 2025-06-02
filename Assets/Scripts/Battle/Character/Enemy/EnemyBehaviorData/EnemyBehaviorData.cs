using RPG_001.Battle.Behaviour;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RPG_001.Battle.Characters.Enemy
{
    public abstract class EnemyBehaviorData : SerializedScriptableObject
    {
        public abstract EnemyBehaviour GetCharacterBehaviour(ICharacter character);
        public abstract EnemySkill GetSkill();
    }
}
