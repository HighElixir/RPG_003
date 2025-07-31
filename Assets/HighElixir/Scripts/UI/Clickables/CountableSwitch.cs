using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;
using System;

namespace HighElixir.UI
{
    public partial class CountableSwitch : MonoBehaviour
    {
        public enum ClickSoundOp
        {
            One, // 常に _clickSound を再生
            Two, // Minus / Plus で別々の音を再生
        }

        [Header("Reference")]
        [SerializeField] private Button _minus;
        [SerializeField] private Button _plus;
        [SerializeField] private TMP_InputField _text;

        [Header("Audio")]
        [SerializeField] private ClickSoundOp _soundOption = ClickSoundOp.One;
        [SerializeField] private AudioClip _clickSound;
        [SerializeField] private AudioClip _disallowedSound;
        [SerializeField] private AudioClip _minusSound;
        [SerializeField] private AudioClip _plusSound;
        private AudioSource _audioSource;

        [Header("Data")]
        [SerializeField] private int _defaultAmount = 0;
        [SerializeField] private int _oneClickChange = 1;
        private int _amount;
        public int min = int.MinValue;
        public int max = int.MaxValue;

        [SerializeField] public UnityEvent<CountableSwitch> _onChanged = new();
        public Func<int, int, bool> AllowChange { get; set; }
        public UnityEvent<CountableSwitch> OnChanged
        {
            get => _onChanged;
            set => _onChanged = value;
        }
        public int CurrentAmount => _amount;

        public bool Change(int amount, bool forceChange = true)
        {
            if (forceChange
                || (amount >= min && amount <= max
                    && (AllowChange == null || AllowChange.Invoke(_amount, amount - _amount))))
            {
                SetAmount(amount);
                return true;
            }
            return false;
        }

        private void SetAmount(int amount)
        {
            _amount = amount;
            _text.text = _amount.ToString();
            _onChanged?.Invoke(this);
        }

        private void Play(AudioClip clip)
        {
            if (clip != null && _audioSource != null)
                _audioSource.PlayOneShot(clip);
        }

        private void Awake()
        {
            // AudioSource 準備
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.playOnAwake = false;

            // 初期値セット
            _amount = _defaultAmount;
            _text.text = _amount.ToString();

            // Minus ボタン
            _minus.onClick.AddListener(() =>
            {
                bool ok = Change(_amount - _oneClickChange, false);
                if (!ok)
                {
                    Play(_disallowedSound);
                }
                else
                {
                    if (_soundOption == ClickSoundOp.Two)
                        Play(_minusSound);
                    else
                        Play(_clickSound);
                }
            });

            // Plus ボタン
            _plus.onClick.AddListener(() =>
            {
                bool ok = Change(_amount + _oneClickChange, false);
                if (!ok)
                {
                    Play(_disallowedSound);
                }
                else
                {
                    if (_soundOption == ClickSoundOp.Two)
                        Play(_plusSound);
                    else
                        Play(_clickSound);
                }
            });

            // テキスト入力確定時
            _text.onEndEdit.AddListener(s =>
            {
                if (!int.TryParse(s, out var temp))
                {
                    _text.text = _amount.ToString();
                    return;
                }
                bool ok = Change(temp, false);
                if (!ok)
                {
                    _text.text = _amount.ToString(); // 失敗時は表示戻し
                }
            });
        }
        public static implicit operator int(CountableSwitch countable) => countable.CurrentAmount;
    }
}
