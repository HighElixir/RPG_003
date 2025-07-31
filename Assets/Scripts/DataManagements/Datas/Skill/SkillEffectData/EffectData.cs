using System;

namespace RPG_003.DataManagements.Datas
{
    [Serializable]
    public struct EffectData
    {
        public enum EffectTarget
        {
            Self,          // 自分に適用
            Target,        // ターゲットに適用
            RandomTarget,  // ランダムなターゲットに適用
            AllAllies,     // 全ての味方に適用
            AllEnemies,     // 全ての敵に適用
            All,          // 全てのユニットに適用
        }
        public string id;
        public string name;
        public string description;
        public string onAddedMessage;
        public string iconPath;
        public string vfxPath; // ":"で複数指定可能
        public float chance; // 効果が発動する確率
        public bool isPositive;
        public EffectTarget target;
        // クラス名を指定する
        // 例：Dotなど
        public string classData;     
        public string data;// データのフォーマットはクラスによって異なる
    }
}