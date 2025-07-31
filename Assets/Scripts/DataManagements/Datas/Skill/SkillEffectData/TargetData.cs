using RPG_003.DataManagements.Enum;
using System;
using UnityEngine;

namespace RPG_003.DataManagements.Datas
{
    [Serializable]
    public struct TargetData
    {
        public bool isSelf;
        public Faction faction;
        public int count;
        public bool isRandom;
        public bool canSelectSameTarget;

        public TargetData(bool isSelf, Faction faction, int count, bool isRandom, bool canSelectSameTarget)
        {
            this.isSelf = isSelf;
            this.faction = faction;
            this.count = Math.Max(count, 1);
            this.isRandom = isRandom; // Fix for CS0171: Assigning _isRandom
            this.canSelectSameTarget = canSelectSameTarget; // Fix for CS0171: Assigning _canSelectSameTarget
        }
    }
}