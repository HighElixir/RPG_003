using RPG_003.Equipments;
using System;
using System.Collections.Generic;

namespace RPG_003.Core
{
    [Serializable]
    public class ItemHolder
    {
        private Dictionary<EquipmentBase, float> _equipDict = new Dictionary<EquipmentBase, float>();

        // = 装備 =
        public float GetEquipCount(EquipmentBase equip) => _equipDict[equip];
        public void AddEquipCount(EquipmentBase equip, float count)
        {
            if (_equipDict.ContainsKey(equip))
                _equipDict[equip] += count;
            else
                _equipDict.Add(equip, count);
        }
        public void RemoveEquipCount(EquipmentBase equip, float count)
        {
            if (_equipDict.ContainsKey(equip))
                _equipDict[equip] -= count;
        }
        //
    }
}