using UnityEngine;
namespace RPG_003.Battle
{
    public partial class BattleManager
    {
        public void EndBattle_Won()
        {
            //Debug.Log("Won");
            GraphicalManager.instance.BattleLog.Add("<color=blue>勝利した！", BattleLog.IconType.Positive);
            GraphicalManager.instance.Sounds.Play(PlaySounds.PlaySound.Win);
            EndBattle();
        }
        public void EndBattle_Lost()
        {
            //Debug.Log("Lost");
            GraphicalManager.instance.BattleLog.Add("<color=red>敗北した,,,", BattleLog.IconType.Negative);
            GraphicalManager.instance.Sounds.Play(PlaySounds.PlaySound.Lose);
            EndBattle();
        }

        public void EndBattle()
        {
            Debug.Log("Battle ended.");
            _isBattleContinue = false;
            foreach (var c in _posManager.GetCharacters()) c.Release();
            _ = BattleExit();
        }

        private void CheckBattleEnd()
        {
            var c = _posManager.AllFactionCount();
            if (c.allies <= 0) EndBattle_Lost();
            if (c.enemies <= 0) EndBattle_Won();
        }
    }
}