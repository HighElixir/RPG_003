using RPG_003.Character;
using RPG_003.Core;
using System.Collections.Generic;

namespace RPG_003.Status
{
    public static class CharacterConvert
    {
        public static StatusData CharacterConvart(this CharacterDataHolder holder)
        {
            var HP = CoreDatas.HP + holder.StatusPoints[StatusAttribute.HP] * CoreDatas.HPPerPoint;
            var MP = CoreDatas.MP + holder.StatusPoints[StatusAttribute.MP] * CoreDatas.MPPerPoint;
            var STR = CoreDatas.STR + holder.StatusPoints[StatusAttribute.STR] * CoreDatas.STRPerPoint;
            var INT = CoreDatas.INT + holder.StatusPoints[StatusAttribute.INT] * CoreDatas.INTPerPoint;
            var SPD = CoreDatas.SPD + holder.StatusPoints[StatusAttribute.SPD] * CoreDatas.SPDPerPoint;
            var LUK = CoreDatas.LUK + holder.StatusPoints[StatusAttribute.LUK] * CoreDatas.LUKPerPoint;
            var DEF = CoreDatas.DEF + holder.StatusPoints[StatusAttribute.DEF] * CoreDatas.DEFPerPoint;
            var MDEF = CoreDatas.MDEF + holder.StatusPoints[StatusAttribute.MDEF] * CoreDatas.MDEFPerPoint;
            var TDS = CoreDatas.TDS + holder.StatusPoints[StatusAttribute.TakeDamageScale] * CoreDatas.TDSPerPoint;
            var CRR = CoreDatas.CRR + holder.StatusPoints[StatusAttribute.CriticalRate] * CoreDatas.CRTPerPoint.rate;
            var CRD = CoreDatas.CRD + holder.StatusPoints[StatusAttribute.CriticalRate] * CoreDatas.CRTPerPoint.bonus;
            var b = LUKtoCrit(LUK);
            CRR += b.rate;
            CRD += b.bonus;
            List<StatusData.ElementDamageScale> Take = new();
            List<StatusData.ElementDamageScale> Give = new();
            foreach (var item in holder.ElementPoints)
            {
                var kvp = CoreDatas.ElementPerPoint;
                Take.Add(new(item.Key, CoreDatas.TES + kvp.take * item.Value));
                Give.Add(new(item.Key, CoreDatas.GES + kvp.give * item.Value));
            }
            var res = new StatusData(holder.Name, HP, MP, STR, INT, SPD, DEF, MDEF, LUK, CRR, CRD, TDS, Take, Give);
            return res;
        }

        public static (float rate, float bonus) LUKtoCrit(float ruk)
        {
            var rate = CoreDatas.CRTPerPoint.rate * (ruk - 100);
            var bonus = CoreDatas.CRTPerPoint.bonus * (ruk - 100);
            return (rate, bonus);
        }
    }
}