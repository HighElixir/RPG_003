using HighElixir;
using HighElixir.UI;
using RPG_003.Core;
using RPG_003.DataManagements.Datas;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG_003.Character
{
    public class CharacterBuilder : MonoBehaviour
    {
        [Serializable]
        public class Container
        {
            public StatusAttribute attribute;
            public Elements elements;
            public CountableSwitch countable;
            public TMP_Text text;// 得られるステータスボーナスを表示するためのやつ

            public void SetPoint(CharacterDataHolder holder)
            {
                if (attribute != StatusAttribute.None)
                {
                    holder.SetStatusPoint(attribute, countable.CurrentAmount);
                }
                else if (elements != Elements.None)
                {
                    holder.SetElememtPoint(elements, countable.CurrentAmount);
                }
            }
            public Container SetStatus(StatusAttribute status)
            {
                attribute = status;
                return this;
            }
            public Container SetElements(Elements elements)
            {
                this.elements = elements;
                return this;
            }
        }
        [SerializeField] private List<Container> _children = new();
        [SerializeField] private TMP_InputField _field;
        [SerializeField] private Button _confirm;
        [SerializeField] private CharacterUIBuilder _ui;
        private HedgeSum _hedgeSum = new();
        private CharacterDataHolder _temp;
        public CharacterDataHolder Temp => _temp;
        public void EnterCreate()
        {
            _temp = new CharacterDataHolder().SetUsablePoints(CoreDatas.UsablePoint);
            _hedgeSum = new HedgeSum().Hedge(0, CoreDatas.UsablePoint).DisallowedNegative(true);
            foreach (var item in _children)
            {
                if (item.countable == null) continue;
                _hedgeSum.Add(item.countable);
                item.countable.OnChanged.AddListener(_ =>
                {
                    item.SetPoint(_temp);
                    _ui.UpdateUI(_children, _temp);
                });
            }
            _ui.SetDelegate(_children);
            _ui.UpdateUI(_children, _temp);
            ResetPoint();
        }

        public void ResetPoint()
        {
            foreach (var item in _children)
            {
                if (item.countable == null) continue;
                item.countable.Change(0);
            }
        }
        public void Save()
        {
            GameDataHolder.instance.Players.Add(_temp);
            Debug.Log("キャラクターをセーブ: " + Temp.Name);
        }
#if UNITY_EDITOR
        //[Button("SetList")]
        private void SetList()
        {
            _children.Clear();
            foreach (var status in EnumWrapper.GetEnumList<StatusAttribute>())
                _children.Add(new Container().SetStatus(status));
            foreach (var element in EnumWrapper.GetEnumList<Elements>())
                _children.Add(new Container().SetElements(element));
        }

        private void Start()
        {
            EnterCreate();
            _field.onEndEdit.AddListener(e =>
            {
                _temp.SetName(e);
                _ui.UpdateStats(_temp);
            });
        }
#endif
    }
}