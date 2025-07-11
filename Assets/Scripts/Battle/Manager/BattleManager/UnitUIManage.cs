using HighElixir.Pool;
using TMPro;
using UnityEngine;

namespace RPG_003.Battle
{
    public partial class BattleManager
    {
        [SerializeField] private TextPool _pool;
        public void SetName(Unit unit)
        {
            unit.GetComponent<UnitUI>().SetText(_pool.Pool.Get());
        }
        public void ReleaceText(TMP_Text text)
        {
            _pool.Pool.Release(text);
        }
    }
}