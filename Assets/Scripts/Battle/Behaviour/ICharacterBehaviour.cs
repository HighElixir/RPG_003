using Cysharp.Threading.Tasks;
using System.Collections;
using System.Threading;

namespace RPG_003.Battle.Behaviour
{
    public interface ICharacterBehaviour
    {
        ICharacterBehaviour Initialize(Unit parent, BattleManager battleManager);

        /// <param name="instant">ターン開始処理や終了処理を飛ばすかどうか(ターン割込みなどでTrue)</param>
        UniTask TurnBehaviour(CancellationToken token, bool instant = false);
        void OnDeath(Unit character);
    }
}