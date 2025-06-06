using System;
using UnityEngine;

namespace RPG_003.Battle.Characters
{
    [Serializable]
    public struct CharacterData
    {
        [SerializeField] private string _name;
        [SerializeField] private float _hp;
        [SerializeField] private float _mp;
        [SerializeField] private float _str;
        [SerializeField] private float _int;
        [SerializeField] private float _spd; // defalt : 100
        [SerializeField] private float _def;
        [SerializeField] private float _mdef;
        [SerializeField] private float _luk;

        public string Name { get => _name; set => _name = value; }
        public float HP => _hp;
        public float MP => _mp;
        public float STR => _str;
        public float INT => _int;
        public float SPD => _spd;
        public float DEF => _def;
        public float MDEF => _mdef;
        public float LUK => _luk;

        public CharacterData(
            string name,
            float hp,
            float mp,
            float str,
            float @int,
            float spd,
            float def,
            float mdef,
            float luk)
        {
            _name = name;
            _hp = hp;
            _mp = mp;
            _str = str;
            _int = @int;
            _spd = spd;
            _def = def;
            _mdef = mdef;
            _luk = luk;
        }
    }
}