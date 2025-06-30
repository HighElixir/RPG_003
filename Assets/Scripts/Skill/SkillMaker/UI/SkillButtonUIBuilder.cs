using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
namespace RPG_003.Skills
{
    public class SkillButtonUIBuilder : MonoBehaviour
    {
        [SerializeField] private SkillBuilder _skillBuilder;
        [SerializeField, BoxGroup("Button")] private Button _confirmButton;
        [SerializeField, BoxGroup("Button")] private Button _cancelButton;
        [SerializeField, BoxGroup("SkillButton")] private Color _canClick;
        [SerializeField, BoxGroup("SkillButton")] private Color _cannotClick;
        [SerializeField, BoxGroup("Stats")] private GameObject _statsObject;
        [SerializeField, BoxGroup("Stats")] private TMP_Text _statsData;
        [SerializeField, BoxGroup("Input")] private TMP_InputField _nameInputField;
        [SerializeField, BoxGroup("Input")] private TMP_InputField _descriptionInputField;
        [SerializeField, BoxGroup("UITemp")] private BasicUI _basicUI;
        [SerializeField, BoxGroup("UITemp")] private ModifierUI _modifierUI;

        private CompositeDisposable _clickDisposables = new CompositeDisposable();
        private SkillUITemp _current;
        private List<SkillBuilder.DataContainer> _dataContainers = new List<SkillBuilder.DataContainer>();
        public void UpdateUI(List<SkillBuilder.DataContainer> containers, SkillMaker.SkillType type, SkillDataHolder holder)
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
                    }).AddTo(_clickDisposables);
                }
                else
                {
                    button.GetComponentInChildren<TMP_Text>().text = data.Name;
                    button
                        .OnClickAsObservable()
                        .Subscribe(_ =>
                    {
                        if (_nameInputField.text == data.Name)
                            _nameInputField.text = "";
                        if (_descriptionInputField.text == data.Description)
                            _descriptionInputField.text = "";
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
                // Add cases for other skill types as needed
                default:
                    Debug.LogWarning("Unsupported skill type: " + type);
                    return;
            }
            ActiveOnly();
            _current.UpdateUI(containers);

            // 各UIの位置を更新
            var (confirmPos, cancelPos) = _current.GetButtonPositions();
            var (namePos, descPos) = _current.GetTextPositions();
            _confirmButton.transform.position = confirmPos;
            _cancelButton.transform.position = cancelPos;
            _statsObject.transform.position = _current.GetStatsPosition();
            _nameInputField.transform.position = namePos;
            _descriptionInputField.transform.position = descPos;

            // 各UIのデータを更新
            if (holder.SkillData != null)
                _statsData.text = holder.ToString();
        }

        // Private
        private void ActiveOnly()
        {
            List<SkillUITemp> temps = new List<SkillUITemp> { _basicUI, _modifierUI };
            foreach (var temp in temps)
            {
                if (temp == _current)
                    temp.Show();
                else
                    temp.Hide();
            }
        }
    }
}