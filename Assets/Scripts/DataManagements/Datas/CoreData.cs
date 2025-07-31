namespace RPG_003.DataManagements.Datas
{
    public static class CoreDatas
    {
        // 基礎ステータス
        public static readonly float HP = 5000f;
        public static readonly float MP = 3200f;
        public static readonly float STR = 1700f;
        public static readonly float INT = 1700f;
        public static readonly float DEF = 1700f;
        public static readonly float MDEF = 1700f;
        public static readonly float SPD = 100f;
        public static readonly float LUK = 100f;
        public static readonly float TDS = 1f; // Take Damage Scale
        public static readonly float TES = 1f; // Take Element Scale
        public static readonly float GES = 1f; // Give Element Scale
        public static readonly float CRR = 0.05f; // Critical Rate
        public static readonly float CRD = 0.5f; // Critical Damage

        // ステータスポイント関連
        public static readonly int UsablePoint = 100; // 初期状態で使用可能なポイント
        public static readonly float HPPerPoint = 300f;
        public static readonly float MPPerPoint = 100f;
        public static readonly float STRPerPoint = 100f;
        public static readonly float INTPerPoint = 100f;
        public static readonly float DEFPerPoint = 100f;
        public static readonly float MDEFPerPoint = 100f;
        public static readonly float SPDPerPoint = 0.5f;
        public static readonly float LUKPerPoint = 0.5f;
        public static readonly (float rate, float bonus) CRTPerPoint = (0.004f, 0.007f);
        public static readonly float TDSPerPoint = -0.003f;
        public static readonly (float take, float give) ElementPerPoint = (0.01f, -0.005f);
    }
}