using System;
using System.Collections.Generic;

namespace HighElixir.StateMachine
{
    // ステートマシン本体のインターフェース
    public interface IStateMachine<T> where T : class
    {
        Dictionary<string, IState<T>> StateMap { get; }  // ステートの一覧（名前と対応するインスタンス）
        (string Key, IState<T> State) CurrentState { get; }                  // 現在のステート

        string DefaultStateKey { get; }

        void SetCondition(Func<string> condition);
        void ChangeState(string newStateKey);         // 即時ステート変更
        void ChangeRequest(string requestKey);        // ステート変更の予約 or 条件付き変更
        void Update();                                // ステートマシンの更新処理（Stayの呼び出しなど）
        void AddState(string newStateKey, IState<T> state);  // ステートの追加
    }
}