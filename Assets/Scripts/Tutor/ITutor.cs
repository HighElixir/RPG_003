using Cysharp.Threading.Tasks;

namespace RPG_003.Tutor
{
    /// <summary>
    /// チュートリアルの挙動を制御する
    /// </summary>
    public interface ITutor
    {
        UniTask Script();
    }
}