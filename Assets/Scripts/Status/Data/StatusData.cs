using HighElixir;
using RPG_003.Status;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG_003.Status
{
    [Serializable]
    public struct StatusData
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
        [SerializeField] private float _spd; // default: 100
        [SerializeField] private float _def;
        [SerializeField] private float _mdef;
        [SerializeField] private float _luk; // default: 100
        [SerializeField] private float _criticalRate; // クリティカル率
        [SerializeField] private float _criticalDamage; // クリティカルダメージ倍率
        [SerializeField] private float _takeDamageScale; // 受けるダメージ倍率
        [SerializeField] private List<ElementDamageScale> _takeElementDamageScale; // 受ける属性ダメージ倍率
        [SerializeField] private List<ElementDamageScale> _giveElementDamageScale; // 与える属性ダメージ倍率

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
        public IReadOnlyList<ElementDamageScale> GiveElementDamageScale => _giveElementDamageScale.AsReadOnly();

        // Updated constructor to initialize all fields
        public StatusData(
            string name,
            float hp,
            float mp,
            float str,
            float intelligence,
            float spd,
            float def,
            float mdef,
            float luk,
            float criticalRate,
            float criticalDamage,
            float takeDamageScale,
            List<ElementDamageScale> takeElementDamageScale,
            List<ElementDamageScale> giveElementDamageScale)
        {
            _name = name;
            _hp = hp;
            _mp = mp;
            _str = str;
            _int = intelligence;
            _spd = spd;
            _def = def;
            _mdef = mdef;
            _luk = luk;
            _criticalRate = criticalRate;
            _criticalDamage = criticalDamage;
            _takeDamageScale = takeDamageScale;
            _takeElementDamageScale = takeElementDamageScale ?? new List<ElementDamageScale>();
            _giveElementDamageScale = giveElementDamageScale ?? new List<ElementDamageScale>();
        }

        #region
        public StatusData SetName(string name)
        {
            _name = name;
            return this;
        }

        public StatusData SetHp(float hp)
        {
            _hp = hp;
            return this;
        }

        public StatusData SetMp(float mp)
        {
            _mp = mp;
            return this;
        }

        public StatusData SetStr(float str)
        {
            _str = str;
            return this;
        }

        public StatusData SetInt(float intelligence)
        {
            _int = intelligence;
            return this;
        }

        public StatusData SetSpd(float spd)
        {
            _spd = spd;
            return this;
        }

        public StatusData SetDef(float def)
        {
            _def = def;
            return this;
        }

        public StatusData SetMDef(float mdef)
        {
            _mdef = mdef;
            return this;
        }

        public StatusData SetLuk(float luk)
        {
            _luk = luk;
            return this;
        }

        public StatusData SetCriticalRate(float criticalRate)
        {
            _criticalRate = criticalRate;
            return this;
        }

        public StatusData SetCriticalDamage(float criticalDamage)
        {
            _criticalDamage = criticalDamage;
            return this;
        }

        public StatusData SetTakeDamageScale(float takeDamageScale)
        {
            _takeDamageScale = takeDamageScale;
            return this;
        }

        public StatusData SetTakeElementDamageScale(List<ElementDamageScale> takeElementDamageScale)
        {
            _takeElementDamageScale = takeElementDamageScale ?? new List<ElementDamageScale>();
            return this;
        }

        public StatusData SetGiveElementDamageScale(List<ElementDamageScale> giveElementDamageScale)
        {
            _giveElementDamageScale = giveElementDamageScale ?? new List<ElementDamageScale>();
            return this;
        }
        public StatusData ListInit()
        {
            if (_takeElementDamageScale == null) _takeElementDamageScale = new();
            if (_giveElementDamageScale == null) _giveElementDamageScale = new();
            _takeElementDamageScale.Clear();
            _giveElementDamageScale.Clear();
            foreach (var item in EnumWrapper.GetEnumList<Elements>())
            {
                _takeElementDamageScale.Add(new ElementDamageScale(item, 1f));
                _giveElementDamageScale.Add(new ElementDamageScale(item, 1f));
            }
            return this;
        }
        #endregion
    }
}