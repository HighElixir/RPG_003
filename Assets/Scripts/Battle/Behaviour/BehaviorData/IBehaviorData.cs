using System.Collections.Generic;

namespace RPG_003.Battle
{
    public interface ISkillBehaviour
    {
        void Initialize();
        List<Skill> Skills { get; }
        AISkillSet GetSkill(Unit parent);
    }
}
