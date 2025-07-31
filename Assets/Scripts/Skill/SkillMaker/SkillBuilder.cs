using HighElixir.Pool;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace RPG_003.Skills
{
    public class SkillBuilder : MonoBehaviour
    {
        public class DataContainer
        {
            public Button Button { get; set; }
            public bool IsUsed { get; set; }
            public SkillData Data { get; set; }
            public int Count { get; set; }

            public DataContainer(Button button, bool isUsed, SkillData data, int count)
            {
                Button = button;
                IsUsed = isUsed;
                Data = data;
                Count = count;
            }
            public override string ToString()
            {
                var sb = new StringBuilder();
                sb.AppendLine($"IsUsed : {IsUsed}");
                sb.AppendLine($"SkillName : {Data.Name}");
                sb.AppendLine($"Count : {Count}");
                return sb.ToString();
            }
        }
        [SerializeField, BoxGroup("Pool")] private Pool<Button> _buttonPool;
        [SerializeField, BoxGroup("Pool")] private Button _prefab;
        [SerializeField, BoxGroup("Pool")] private Transform _container;
        [SerializeField, BoxGroup("InputData")] private TMP_InputField _name;
        [SerializeField, BoxGroup("InputData")] private TMP_InputField _description;
        [SerializeField, BoxGroup("Button")] private Button _confirm;
        [SerializeField, BoxGroup("Button")] private Button _delete;
        [SerializeField] private SkillMaker _director;
        [SerializeField] private SkillUIBuilder _ui;
        [SerializeField] private SkillGetter _getter;
        private ReactiveProperty<SkillHolder> _temp = new();
        private IDisposable _unSubscriber;
        private SkillMaker.SkillType _currentWork;
        private List<DataContainer> _buttons = new();

        public Action<SkillHolder> OnComplete { get; set; }
        public IObservable<SkillHolder> Temp => _temp;
        public void Init()
        {
            _buttonPool = new(_prefab, 100, _container);
            if (_unSubscriber != null)
                _unSubscriber.Dispose();
            _director = GetComponent<SkillMaker>();
            _ui = GetComponent<SkillUIBuilder>();
            _unSubscriber = _director.Current.Subscribe(Next).AddTo(this);
            Temp.Subscribe(temp =>
            {
                if (temp == null || temp.SkillData == null)
                {
                    _name.text = string.Empty;
                    _description.text = string.Empty;
                }
                else
                {
                    _name.text = temp.Name;
                    _description.text = temp.Desc;
                }
            }).AddTo(this);
            // Initialize input fields
            _name.onEndEdit.RemoveAllListeners();
            _description.onEndEdit.RemoveAllListeners();
            _name.onEndEdit.AddListener((s) =>
            {
                if (_temp != null)
                {
                    _temp.Value.SetCustomName(s);
                    _ui.UpdateUI(_buttons, _currentWork, _temp.Value);
                }
            });
            _description.onEndEdit.AddListener((s) =>
            {
                if (_temp != null)
                {
                    _temp.Value.SetCustomDesc(s);
                    _ui.UpdateUI(_buttons, _currentWork, _temp.Value);
                }
            });

            // Initialize button
            _confirm.onClick.RemoveAllListeners();
            _delete.onClick.RemoveAllListeners();
            _confirm.onClick.AddListener(() =>
            {
                if (_temp != null)
                {
                    if (!_temp.Value.IsValid(out var m))
                    {
                        Debug.LogWarning(m);
                    }
                    else
                    {
                        _getter.Save();
                        OnComplete?.Invoke(_temp.Value);
                        _temp.Value = null;
                    }
                }
            });
            _delete.onClick.AddListener(() =>
            {
                if (_temp != null)
                {
                    Next(_currentWork);
                }
            });
        }

        public void Next(SkillMaker.SkillType value)
        {
            _currentWork = value;
            Debug.Log($"ChangeMode: {value}");
            BreakHolder(_temp.Value);
            switch (value)
            {
                case SkillMaker.SkillType.Basic:
                    _temp.Value = new BasicHolder();
                    break;
                case SkillMaker.SkillType.Modifies:
                    _temp.Value = new ModifierHolder();
                    break;
                case SkillMaker.SkillType.Smith:
                    _temp.Value = new SmithHolder();
                    break;
                default:
                    _temp.Value = null;
                    break;
            }
            if (value == SkillMaker.SkillType.None)
            {
                ReleaseButtons();
                _ui.UpdateUI(new List<DataContainer>(), value, _temp.Value);
            }
            else
            {
                SetButton();
                _ui.UpdateUI(_buttons, value, _temp.Value);
            }
        }

        private void SetButton()
        {
            ReleaseButtons();
            var dic = _getter.GetByType(_currentWork);
            Debug.Log($"SetButton: {dic.Count}");
            // セット可能なスキルデータを取得
            foreach (var kvp in dic.ToList())
            {
                if (kvp.Value <= 0) continue; // スキルデータがない場合はスキップ
                var b = _buttonPool.Get();
                b.onClick.AddListener(() =>
                {
                    if (dic.TryGetValue(kvp.Key, out int count) && count >= 1 && _temp.Value.CanSetSkillData(kvp.Key))
                    {
                        if (_temp.Value.IsNeedReplace(kvp.Key, out var oldItems))
                        {
                            foreach (var item in oldItems)
                            {
                                if (item == null) continue;
                                _getter.Add(item, 1);
                                _temp.Value.RemoveSkillData(item);
                            }
                        }
                        _getter.Remove(kvp.Key, 1);
                        _temp.Value.SetSkillData(kvp.Key);
                        Debug.Log($"Set SkillData: {kvp.Key.Name}, having {dic[kvp.Key]}");
                        SetButton();
                        _ui.UpdateUI(_buttons, _currentWork, _temp.Value);
                    }
                });
                var c = new DataContainer(b, false, kvp.Key, kvp.Value);
                Debug.Log(c.ToString());
                _buttons.Add(c);
            }

            Debug.Log(_temp.Value.GetSkillDatas().Count + " skills set in temp holder.");
            // スキルにセット済みのスキルデータをボタンにセット
            foreach (var item in _temp.Value.GetSkillDatas())
            {
                if (item == null) continue;
                var b = _buttonPool.Get();
                b.onClick.AddListener(() =>
                {
                    if (_temp.Value.RemoveSkillData(item, out var list))
                    {
                        _getter.Add(item, 1); // スキルデータを戻す
                        foreach (var item2 in list)
                        {
                            Debug.Log($"{item2.Name} is Removed.");
                            _getter.Add(item2, 1);
                        }
                    }
                    SetButton(); // Update buttons after breaking the skill data
                    _ui.UpdateUI(_buttons, _currentWork, _temp.Value);
                });
                var c = new DataContainer(b, true, item, 1);
                Debug.Log(c.ToString());
                _buttons.Add(c);
            }
        }

        private void ReleaseButtons()
        {
            foreach (var item in _buttons)
            {
                ReleaseButton(item.Button);
            }
            _buttons.Clear();
            Debug.Log("ReleaseButtons Completed");
        }
        private void BreakHolder(SkillHolder skillHolder)
        {
            if (skillHolder == null || skillHolder.SkillData == null) return;
            var list = skillHolder.GetSkillDatas();
            foreach (var data in list)
            {
                if (data == null) continue;
                Debug.Log(data.ToString());
                if (_getter.Datas.ContainsKey(data))
                {
                    _getter.Datas[data] += 1;
                }
                else
                {
                    _getter.Datas.Add(data, 1);
                }
            }
        }
        private void ReleaseButton(Button button)
        {
            if (button == null) return;
            button.onClick.RemoveAllListeners();
            _buttonPool.Release(button);
        }
        //
        [Button("Log")]
        private void DebugLogs()
        {
            Debug.Log(_temp.ToString());
        }
        [Button("Reset")]
        private void Awake()
        {
            Init();
        }
    }
}
