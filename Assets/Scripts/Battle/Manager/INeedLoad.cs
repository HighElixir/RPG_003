using Cysharp.Threading.Tasks;

namespace RPG_003.Battle
{
    /// <summary>
    /// 事前のロードが必要なクラスに実装するインターフェース
    /// </summary>
    public interface INeedLoad
    {
        UniTask LoadData();
    }
    public interface INeedUnload
    {
        UniTask UnloadData();
    }
}