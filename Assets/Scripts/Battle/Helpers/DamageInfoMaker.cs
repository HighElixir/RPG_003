using RPG_003.DataManagements.Datas;
using RPG_003.DataManagements.Datas.Helper;
using UnityEngine;

namespace RPG_003.Battle
{
    public static class DamageInfoMaker
    {
        public static DamageInfo MakeDamageInfo(this DamageData data, Unit target, Unit source)
        {
            var d = new DamageInfo(source, target, 0);

            // 固定ダメージ
            float baseDamage = data.fixedAmount;
            //Debug.Log($"[DamageCalc] 固定ダメージ: {baseDamage}");

            float damage = baseDamage;

            if (source != null)
            {
                // ソースのステータス量取得
                float amount;
                switch (data.type)
                {
                    case StatusAttribute.HP:
                        amount = source.StatusManager.HP;
                        break;
                    case StatusAttribute.MaxHP:
                        amount = source.StatusManager.MaxHP;
                        break;
                    case StatusAttribute.MP:
                        amount = source.StatusManager.GetStatusAmount(StatusAttribute.MP).currentAmount;
                        break;
                    default:
                        amount = source.StatusManager.GetStatusAmount(data.type).ChangedMax;
                        break;
                }
                // Debug.Log($"[DamageCalc] ステータス量 ({data.type}): {amount}");

                // 追加ダメージ = amount * data.amount * data.variance
                var rand = UnityEngine.Random.Range(1f - data.variance, 1f + data.variance);
                float addDamage = amount * data.amount * rand;
                //Debug.Log($"[DamageCalc] 追加ダメージ = {amount} * {data.amount} * {rand} = {addDamage}");

                damage += addDamage;
                //Debug.Log($"[DamageCalc] ダメージ合計（属性補正前）: {damage}");

                // 属性バフ
                if (source.StatusManager.TryGetStatus(data.element.GetStatusFromElement(true), out var e))
                {
                    float elementMul = 1 + e.ChangedMax;
                    //Debug.Log($"[DamageCalc] 属性補正率 = 1 + {e.ChangedMax} = {elementMul}");
                    damage *= elementMul;
                    //Debug.Log($"[DamageCalc] ダメージ合計（属性補正後）: {damage}");
                }

                // クリティカル判定
                if (data.IsCritical(source))
                {
                    float critRate = source.StatusManager.GetStatusAmount(StatusAttribute.CriticalDamage).ChangedMax;
                    float critMul = 1 + critRate;
                    //Debug.Log($"[DamageCalc] クリティカル補正率 = 1 + {critRate} = {critMul}");

                    damage *= critMul;
                    d.IsCritical = true;
                    //Debug.Log($"[DamageCalc] 最終ダメージ（クリティカル）: {damage}");
                }

                d.Damage = (float)((int)damage);
            }

            d.Elements = data.element;
            d.AmountAttribute = data.amountAttribute;
            return d;
        }

        public static DamageInfo ResistDamage(this DamageInfo source)
        {
            // クローンしてスタート
            DamageInfo d = source.Clone() as DamageInfo;

            // 属性がConsume(消費)の時は何もしない
            if (source.AmountAttribute.HasFlag(AmountAttribute.Consume)) return d;
            // 元ダメージ
            float original = d.Damage;
            //Debug.Log($"[ResistCalc] 元のダメージ: {original}");

            // 防御力取得
            StatusAmount def;
            if (d.AmountAttribute == AmountAttribute.Magic)
            {
                def = d.Target.StatusManager.GetStatusAmount(StatusAttribute.MDEF);
            }
            else
            {
                def = d.Target.StatusManager.GetStatusAmount(StatusAttribute.DEF);
            }
            //Debug.Log($"[ResistCalc] 防御力: {def.ChangedMax}");

            // 防御後ダメージ
            float afterDef = original - def.ChangedMax;
            //Debug.Log($"[ResistCalc] 防御後ダメージ = {original} - {def.ChangedMax} = {afterDef}");

            // ダメージ耐性による減少
            float resistRate = source.Target.StatusManager.GetStatusAmount(StatusAttribute.TakeDamageScale).ChangedMax;
            //Debug.Log($"[ResistCalc] ダメージ耐性率: {resistRate}");
            float afterResist = afterDef * (resistRate);
            //Debug.Log($"[ResistCalc] 耐性後ダメージ = {afterDef} * {resistRate} = {afterResist}");

            // 属性耐性による減少
            float elemResist = source.Target.StatusManager
                .TryGetStatus(source.Elements.GetStatusFromElement(false), out var amount) ? amount.ChangedMax : 1f;
            //Debug.Log($"[ResistCalc] 属性耐性率: {elemResist}");
            float afterElemResist = afterResist * elemResist;
            //Debug.Log($"[ResistCalc] 属性耐性後ダメージ = {afterResist} * {elemResist} = {afterElemResist}");

            // 最低ダメージ制限（元の1/4）
            float minDamage = original * 0.25f;
            //Debug.Log($"[ResistCalc] 最小ダメージ制限: 元の0.25倍 = {minDamage}");
            float clamped = Mathf.Max(afterElemResist, minDamage);
            //Debug.Log($"[ResistCalc] クランプ後ダメージ = Max({afterElemResist}, {minDamage}) = {clamped}");

            // 整数化してセット
            d.Damage = (float)((int)clamped);
            //Debug.Log($"[ResistCalc] 最終ダメージ (intキャスト後) = {d.Damage}");

            return d;
        }

        private static bool IsCritical(this DamageData data, Unit source)
        {
            if (data.criticalRate <= 0f) return false;
            var rand = Random.Range(0f, 1f);
            return rand < data.criticalRate + source.StatusManager.GetStatusAmount(StatusAttribute.CriticalRate).ChangedMax;
        }
    }
}
