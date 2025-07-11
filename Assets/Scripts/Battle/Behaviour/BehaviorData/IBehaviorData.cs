using System.Collections.Generic;
using RPG_003.Battle.Behaviour;

namespace RPG_003.Battle
{
    public interface ISkillBehaviour
    {
        void Initialize();
        List<Skill> Skills { get; }
        Skill GetSkill(Unit parent);
    }
}
