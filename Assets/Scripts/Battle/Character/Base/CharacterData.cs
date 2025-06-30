using RPG_003.Status;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG_003.Battle
{
    [Serializable]
    public struct CharacterData
    {
        [Serializable]
        public struct ElementDamageScale
        {
            public Elements element;
            public float scale;
            public ElementDamageScale(Elements element, float scale = 1f)
            {
                this.element = element;
                this.scale = scale;
            }
        }
        [SerializeField] private string _name;
        [SerializeField] private float _hp;
        [SerializeField] private float _mp;
        [SerializeField] private float _str;
        [SerializeField] private float _int;
        [SerializeField] private float _spd; // defalt : 100
        [SerializeField] private float _def;
        [SerializeField] private float _mdef;
        [SerializeField] private float _luk; // defalt : 100
        [SerializeField] private float _criticalRate; // クリティカル率
        [SerializeField] private float _criticalDamage; // クリティカルダメージ倍率
        [SerializeField] private float _takeDamageScale; // 受けるダメージ倍率
        [SerializeField] private List<ElementDamageScale> _takeElementDamageScale; // 受ける属性ダメージ倍率
        public string Name { get => _name; set => _name = value; }
        public float HP => _hp;
        public float MP => _mp;
        public float STR => _str;
        public float INT => _int;
        public float SPD => _spd;
        public float DEF => _def;
        public float MDEF => _mdef;
        public float LUK => _luk;
        public float CR => _criticalRate;
        public float CRDamage => _criticalDamage;
        public float LUKToCR => _luk / 2000f;
        public float LUKToCRDamage => _luk / 200f;
        public float TakeDamageScale => _takeDamageScale;
        public IReadOnlyList<ElementDamageScale> TakeElementDamageScale => _takeElementDamageScale.AsReadOnly();
    }
}