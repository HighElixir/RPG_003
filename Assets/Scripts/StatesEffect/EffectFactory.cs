using RPG_003.DataManagements.Datas;
using System;

namespace RPG_003.StatesEffect
{
    public static class EffectFactory
    {
        public static StatesEffectBase CreateEffect(EffectData effectData)
        {
            var typeName = $"RPG_003.StatesEffect.{effectData.classData}";
            var type = Type.GetType(typeName);

            if (type == null)
            {
                throw new InvalidOperationException($"クラス {effectData.classData} が見つからなかったよ！");
            }

            // インスタンス生成
            if (Activator.CreateInstance(type) is not StatesEffectBase instance)
            {
                throw new InvalidOperationException($"クラス {effectData.classData} は StatesEffectBase を継承してないか、インスタンス化できないよ！");
            }
            // 初期化
            instance.SetData(effectData);

            return instance;
        }
    }
}