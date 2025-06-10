using RPG_003.Battle.Behaviour;
using Sirenix.OdinInspector;

namespace RPG_003.Battle.Characters.Enemy
{
    public abstract class EnemyBehaviorData : SerializedScriptableObject
    {
        public abstract AIBehavior GetCharacterBehaviour();
        public abstract EnemySkill GetSkill();
    }
}
