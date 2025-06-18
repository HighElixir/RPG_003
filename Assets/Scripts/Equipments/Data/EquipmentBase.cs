using RPG_003.Status;

using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG_003.Equipments
{
    public class EquipmentBase : ScriptableObject
    {
        [Serializable]
        public struct EquipmentState
        {
            public StatusAttribute target;
            public float scale; // targetにかかる係数（scale同士は加算）
            public float fix;
        }
        [SerializeField] private string _name;
        [SerializeField] private string _description;
        [SerializeField] private EquiptablePosition _position;
        [SerializeField] private Sprite _icon;
        [SerializeField] private float _amount; // 購入金額
        [SerializeField] private float _dropChance;
        [SerializeField] List<EquipmentState> _equipmentStates;

        public string Name => _name;
        public string Description => _description;
        public Sprite Icon => _icon;
        public float amount => _amount;
        public float DroppChance => _dropChance;
        public List<EquipmentState> EquipmentStates => _equipmentStates;
    }
}