using Cysharp.Threading.Tasks;
using DG.Tweening;
using HighElixir.Pool;
using Lean.Gui;
using RPG_003.Core;
using RPG_003.Skills;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace RPG_003.Equipments
{
    public class EquipmentManager : MonoBehaviour
    {
        [SerializeField] private LeanWindow _window;
        [SerializeField, BoxGroup("Pool")] private Button _prefab;
        [SerializeField, BoxGroup("Pool")] private int _poolSize;
        [SerializeField, BoxGroup("Pool")] private RectTransform _container;
        [SerializeField, BoxGroup("Player")] private Button[] _playerButtons = new Button[4];
        [SerializeField, BoxGroup("UI")] private GridLayoutGroup _skillCont;
        [SerializeField, BoxGroup("UI")] private PlayerIndicator _indi;
        private Pool<Button> _pool;
        private Dictionary<Button, IDisposable> _buttons = new();

        public bool SetSkill { get; private set; } = false;
        private SkillHolder _holder;
        public void BuildButtons()
        {
            // 初期化
            _indi.gameObject.SetActive(false);
            SetSkill = false;

            // プレイヤー設定
            var players = GameDataHolder.instance.Players.Data;
            for (int i = 0; i < players.Count; i++)
            {
                var p = players[i];
                _playerButtons[i].GetComponentInChildren<TMP_Text>().SetText(p.Name);
                _playerButtons[i].onClick.AsObservable().Subscribe(_ =>
                {
                    // プレーヤーの情報表示クラスに情報を渡す
                    _indi.gameObject.SetActive(!_indi.gameObject.activeInHierarchy);
                    _indi.SetPlayer(p);
                }).AddTo(this);
            }

            // スキルの描画、クリックの設定
            // 表示中のボタンをすべてリリース
            foreach (var b in _buttons)
            {
                _pool.Release(b.Key);
                b.Value.Dispose();
            }
            _buttons.Clear();
            var skills = GameDataHolder.instance.Skills;
            foreach (var skill in skills)
            {
                var b = _pool.Get();
                var d = b.onClick.AsObservable().Subscribe(hoge =>
                {
                    SetSkill = true;
                    _ = ShakeButton(b);
                    _holder = skill;
                }).AddTo(this);
                b.GetComponentInChildren<TMP_Text>().SetText(skill.Name);
                b.image.sprite = skill.Icon;
                b.transform.parent = _skillCont.transform;
                _buttons[b] = d;
            }
        }

        // スキル設定が完了、キャンセルされるまでアニメーションを続ける
        public async UniTask ShakeButton(Button button)
        {
            var defaultRotation = button.transform.rotation;
            var tween = button.transform.DOShakeRotation(2, strength:15).SetLoops(-1);
            await UniTask.WaitWhile(() => SetSkill);
            tween.Kill();
            button.transform.rotation = defaultRotation;
        }

        // PlayerIndicatorから受け取る想定
        // 現在受け取れるかどうか（SetSkillがTrueかどうか）は向こうで判断
        public SkillHolder ReceiveSkill()
        {
            SetSkill = false;
            return _holder;
        }
        private void Awake()
        {
            _pool = new Pool<Button>(_prefab, _poolSize, _container, true);
            _window.OnOn.AsObservable().Subscribe(_ => BuildButtons());
        }
    }
}