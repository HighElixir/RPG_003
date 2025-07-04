﻿using System.Collections;

namespace RPG_003.Battle.Behaviour
{
    public interface ICharacterBehaviour
    {
        void Initialize(CharacterObject parent, BattleManager battleManager);

        /// <param name="instant">ターン開始処理や終了処理を飛ばすかどうか(ターン割込みなどでTrue)</param>
        IEnumerator TurnBehaviour(bool instant = false);
        void OnDeath(CharacterObject character);
    }
}