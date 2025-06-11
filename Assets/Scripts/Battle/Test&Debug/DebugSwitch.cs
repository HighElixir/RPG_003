using HighElixir.Utilities;
using UnityEngine;

#if UNITY_EDITOR
namespace RPG_003.Battle
{
    public class DebugSwitch : SingletonBehavior<DebugSwitch>
    {
        [Tooltip("スキルをあらゆる制約を無視して常に発動できるようにする")]
        public bool isThought_cost; 
    }
}
#endif