using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
namespace RPG_003.Skills
{
    public class SkillUIBuilder : MonoBehaviour
    {
        [SerializeField] private SkillBuilder _skillBuilder;
        [SerializeField, BoxGroup("Button")] private Button _confirmButton;
        [SerializeField, BoxGroup("Button")] private Button _cancelButton;
        [SerializeField, BoxGroup("SkillButton")] private Color _canClick;
        [SerializeField, BoxGroup("SkillButton")] private Color _cannotClick;
        [SerializeField, BoxGroup("Stats")] private GameObject _statsObject;
        [SerializeField, BoxGroup("Stats")] private TMP_Text _statsData;
        [SerializeField, BoxGroup("Stats")] private TMP_Text _chipStatus;
        [SerializeField, BoxGroup("Input")] private TMP_InputField _nameInputField;
        [SerializeField, BoxGroup("Input")] private TMP_InputField _descriptionInputField;
        [SerializeField, BoxGroup("UITemp")] private BasicUI _basicUI;
        [SerializeField, BoxGroup("UITemp")] private ModifierUI _modifierUI;
        [SerializeField, BoxGroup("UITemp")] private SmithUI _smithUI;

        private CompositeDisposable _clickDisposables = new CompositeDisposable();
        private SkillUITemp _current;
        private List<SkillBuilder.DataContainer> _dataContainers = new List<SkillBuilder.DataContainer>();
        public void UpdateUI(List<SkillBuilder.DataContainer> containers, SkillMaker.SkillType type, SkillHolder holder)
        {
            // ボタンの設定
            _dataContainers = containers;
            _clickDisposables.Clear();
            foreach (var item in _dataContainers)
            {
                var button = item.Button;
                var data = item.Data;
                var itemCount = item.Count;
                button.image.sprite = data.DefaultIcon;
                if (!item.IsUsed)
                {
                    button.GetComponentInChildren<TMP_Text>().text = data.Name + "(" + itemCount + ")";
                    item.Button.image.color = holder != null && holder.CanSetSkillData(item.Data) ? _canClick : _cannotClick;
                    button
                        .OnClickAsObservable()
                        .Subscribe(_ =>
                    {
                        if (_nameInputField.text.Length == 0 || _nameInputField.text == holder.Name)
                            _nameInputField.text = data.Name;
                        if (_descriptionInputField.text.Length == 0 || _descriptionInputField.text == holder.Desc)
                            _descriptionInputField.text = data.Description;

                        _chipStatus.SetText(item.Data.ToString());
                    }).AddTo(_clickDisposables);
                }
                else
                {
                    button.GetComponentInChildren<TMP_Text>().text = data.Name;
                    button.image.color = _canClick;
                    button
                        .OnClickAsObservable()
                        .Subscribe(_ =>
                    {
                        if (_nameInputField.text == data.Name)
                            _nameInputField.text = "";
                        if (_descriptionInputField.text == data.Description)
                            _descriptionInputField.text = "";

                        _chipStatus.SetText(item.Data.ToString());
                        UpdateUI(containers, type, holder);
                    }).AddTo(_clickDisposables);
                }
            }
            Debug.Log($"UpdateUI: {type} with {containers.Count} buttons");

            // UI 切り替え
            switch (type)
            {
                case SkillMaker.SkillType.Basic:
                    _current = _basicUI;
                    break;
                case SkillMaker.SkillType.Modifies:
                    _current = _modifierUI;
                    break;
                case SkillMaker.SkillType.Smith:
                    _current = _smithUI;
                    break;
                // Add cases for other skill types as needed
                default:
                    _current = null;
                    break;
            }
            ActiveOnly();

            if (_current == null)
            {
                SetActiveAll(false);
                _nameInputField.text = "";
                _descriptionInputField.text = "";
                _chipStatus.SetText("");
                return;
            }
            _current?.UpdateUI(containers);
            SetActiveAll(true);
            // 各UIの位置を更新
            var (confirmPos, cancelPos) = _current.ButtonPositions;
            _confirmButton.transform.position = confirmPos;
            _cancelButton.transform.position = cancelPos;
            var (namePos, descPos) = _current.TextPositions;
            _nameInputField.transform.position = namePos;
            _descriptionInputField.transform.position = descPos;
            _statsObject.transform.position = _current.StatsPosition;
            _chipStatus.transform.position = _current.ChipStatsPosition;

            // 各UIのデータを更新
            if (holder.SkillData != null)
                _statsData.text = holder.ToString();
        }

        // Private
        private void ActiveOnly()
        {
            List<SkillUITemp> temps = new List<SkillUITemp> { _basicUI, _modifierUI, _smithUI };
            foreach (var temp in temps)
            {
                if (temp == _current)
                    temp.Show();
                else
                    temp.Hide();
            }
        }

        public void SetActiveAll(bool active)
        {
            _confirmButton.gameObject.SetActive(active);
            _cancelButton.gameObject.SetActive(active);
            _statsObject.gameObject.SetActive(active);
            _nameInputField.gameObject.SetActive(active);
            _descriptionInputField.gameObject.SetActive(active);
            _chipStatus.gameObject.SetActive(active);
        }
    }
}