namespace HighElixir.StateMachine
{

    /// <summary>
    /// 状態（ステート）ごとのインターフェース。
    /// 各ステート（行動パターン）にこのインターフェースを実装させる。
    /// 返り値のboolは処理の成功／失敗を示す。
    /// </summary>
    public interface IState<TParent> where TParent : class
    {
        // ステートに入ったときの処理（失敗したらfalseを返してエラーハンドリングへ）
        bool Enter(IState<TParent> previousState, TParent parent);

        // ステート中に毎フレーム呼ばれる処理（falseならログ出すけど続行）
        bool Stay(TParent parent);

        // ステートから抜けるときの処理（falseでもログ出して続ける）
        bool Exit(IState<TParent> nextState, TParent parent);
    }
}
