using Unity.VisualScripting;
using UnityEngine.WSA;

namespace RPG_003.Skills
{
    public static class MatchSkillType
    {
        public static bool IsMatch(this SkillData data, SkillDataHolder holder)
        {
            var paramA = (data is SmithChip || data is SkillSlotData) && holder is SmithHolder;
            var paramB = (data is ModifierData || data is AddonData) && holder is ModifierHolder;
            var paramC = data is BasicData && holder is BasicHolder;
            return paramA || paramB || paramC;
        }

        public static bool IsMatch(this SkillData data, SkillType type)
        {
            var paramA = (data is SmithChip || data is SkillSlotData) && type == SkillType.Smith;
            var paramB = (data is ModifierData || data is AddonData) && type == SkillType.Modifies;
            var paramC = data is BasicData && type == SkillType.Basic;
            return paramA || paramB || paramC;
        }
    }
}