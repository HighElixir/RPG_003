using HighElixir;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG_003.DataManagements.Datas
{
    // フォーマット
    // HP, MP, STR, INT, SPD, DEF, MDEF, LUK, CR, CRDamage, TakeDamageScale, GiveElementDamageScale, TakeElementDamageScale
    // 例: 1000,500,150,100,120,80,60,200,0.1,1.5,1.0,(Fire:1.2:0.9, Dark:0.9:1.2, Water:99:99)
    // 属性ダメージの未入力分については1.0をデフォルト値とする
    // 半角・全角スペースは読み込み時に削除される
    [Serializable]
    public struct StatusData
    {
        private static readonly Stack<Elements> _elementStack = new Stack<Elements>(EnumWrapper.GetEnumList<Elements>());
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

        // Chain
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
            foreach (var item in _elementStack)
            {
                _takeElementDamageScale.Add(new ElementDamageScale(item, 1f));
                _giveElementDamageScale.Add(new ElementDamageScale(item, 1f));
            }
            return this;
        }
        #endregion

        // CSV to StatusData
        public static StatusData FromCsv(string csvLine)
        {
            var values = csvLine.Split(',');
            if (values.Length < 12) throw new ArgumentException("CSV line does not contain enough values.");
            string name = values[0].Trim();
            float hp = float.Parse(values[1].Trim());
            float mp = float.Parse(values[2].Trim());
            float str = float.Parse(values[3].Trim());
            float intelligence = float.Parse(values[4].Trim());
            float spd = float.Parse(values[5].Trim());
            float def = float.Parse(values[6].Trim());
            float mdef = float.Parse(values[7].Trim());
            float luk = float.Parse(values[8].Trim());
            float criticalRate = float.Parse(values[9].Trim());
            float criticalDamage = float.Parse(values[10].Trim());
            float takeDamageScale = float.Parse(values[11].Trim());
            List<ElementDamageScale> takeElementDamageScale = new();
            List<ElementDamageScale> giveElementDamageScale = new();
            for (int i = 12; i < values.Length; i++)
            {
                var elementData = values[i].Trim().Split(':');
                if (elementData.Length >= 2)
                {
                    Elements element;
                    if (Enum.TryParse(elementData[0], out element))
                    {
                        takeElementDamageScale.Add(new ElementDamageScale(element, float.Parse(elementData[1])));
                    }
                    if (elementData.Length == 3)
                    {
                        giveElementDamageScale.Add(new ElementDamageScale(element, float.Parse(elementData[2])));
                    }
                }
            }
            return new StatusData(name, hp, mp, str, intelligence, spd, def, mdef, luk, criticalRate, criticalDamage, takeDamageScale, takeElementDamageScale, giveElementDamageScale);
        }
    }
}