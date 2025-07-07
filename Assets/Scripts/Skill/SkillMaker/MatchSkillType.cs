namespace RPG_003.Skills
{
    public static class MatchSkillType
    {
        public static bool IsMatch(this SkillData data, SkillHolder holder)
        {
            var paramA = (data is SmithChip || data is SkillSlotData) && holder is SmithHolder;
            var paramB = (data is ModifierData || data is AddonData) && holder is ModifierHolder;
            var paramC = data is BasicData && holder is BasicHolder;
            return paramA || paramB || paramC;
        }

        public static bool IsMatch(this SkillData data, SkillMaker.SkillType type)
        {
            var paramA = (data is SmithChip || data is SkillSlotData) && type == SkillMaker.SkillType.Smith;
            var paramB = (data is ModifierData || data is AddonData) && type == SkillMaker.SkillType.Modifies;
            var paramC = data is BasicData && type == SkillMaker.SkillType.Basic;
            return paramA || paramB || paramC;
        }

        public static bool IsBasic(this SkillData data)
        {
            return data is BasicData;
        }
        public static bool IsModifier(this SkillData data)
        {
            return data is ModifierData;
        }
        public static bool IsAddon(this SkillData data)
        {
            return data is AddonData;
        }
        public static bool IsSlot(this SkillData data)
        {
            return data is SkillSlotData;
        }
        public static bool IsEffect(this SkillData data)
        {
            return data is EffectChip;
        }
        public static bool IsCost(this SkillData data)
        {
            return data is CostChip;
        }
        public static bool IsTarget(this SkillData data)
        {
            return data is TargetChip;
        }
    }
}