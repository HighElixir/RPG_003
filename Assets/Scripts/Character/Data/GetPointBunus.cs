using RPG_003.DataManagements.Datas;

namespace RPG_003.Character
{
    public static class GetPointBonus
    {
        public static float GetPoint(this StatusAttribute attribute)
        {
            return attribute switch
            {
                StatusAttribute.HP => CoreDatas.HPPerPoint,
                StatusAttribute.MP => CoreDatas.MPPerPoint,
                StatusAttribute.STR => CoreDatas.STRPerPoint,
                StatusAttribute.INT => CoreDatas.INTPerPoint,
                StatusAttribute.DEF => CoreDatas.DEFPerPoint,
                StatusAttribute.MDEF => CoreDatas.MDEFPerPoint,
                StatusAttribute.LUK => CoreDatas.LUKPerPoint,
                StatusAttribute.SPD => CoreDatas.SPDPerPoint,
                StatusAttribute.TakeDamageScale => CoreDatas.TDSPerPoint,
                _ => 0,
            };
        }
    }
}