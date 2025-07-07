using RPG_003.Battle;
using System.Collections.Generic;

namespace RPG_003.Skills
{
    public static class SkillConverter
    {
        public static List<SkillDataInBattle> ConvertList(this List<SkillHolder> skills)
        {
            var list = new List<SkillDataInBattle>();
            foreach (var item in skills)
            {
                list.Add(item.ConvartData());
            }
            return list;
        }
    }
}