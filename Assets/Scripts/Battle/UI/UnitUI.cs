using DG.Tweening;
using TMPro;
using UniRx.Triggers;
using UnityEngine;
using System.Linq;
using UniRx;

namespace RPG_003.Battle
{
    public class UnitUI : MonoBehaviour
    {
        [Tooltip("ワールド座標で移動させたい先の位置")]
        [SerializeField] Vector3 _actionMove = Vector3.zero;
        [Tooltip("移動にかける時間(秒)")]
        [SerializeField] float _moveDuration = 0.5f;
        [Tooltip("到達後に待つ時間(秒)")]
        [SerializeField] float _waitDuration = 1f;
        [Tooltip("シェイクの強さ")]
        [SerializeField] float _strangth = 0.4f;
        [SerializeField] private Vector2 _nameDelta = Vector2.zero;
        private Vector3 _originalPos;
        private Sequence _sequence;
        private TMP_Text _text;
        private Camera _camera;

        // Text
        public void SetText(TMP_Text text)
        {
            _text = text;
            _text.SetText(GetComponent<Unit>().Data.Name);
        }
        // DOTWEEN
        public void Shake()
        {
            Init();

            _sequence = DOTween.Sequence()
                .Append(transform.DOShakePosition(2f, _strangth));
        }

        public void Action()
        {
            Init();

            Vector3 to = _originalPos + (GetComponent<Unit>().Faction == Factions.Faction.Ally ? -_actionMove : _actionMove);

            // 少し前にでる → 少し待機 → 戻る
            _sequence = DOTween.Sequence()
                .Append(transform.DOMove(to, _moveDuration))
                .AppendInterval(_waitDuration)
                .Append(transform.DOMove(_originalPos, _moveDuration));
        }

        private void Init()
        {
            var unit = GetComponent<Unit>();
            _originalPos = unit.BattleManager.TransformHelper.GetPosition(unit.Position);
            if (_sequence != null && _sequence.IsActive())
            {
                // 既存シーケンスはキャンセル
                _sequence.Kill();
                transform.position = _originalPos;
            }
        }
        // UNITY
        private void Awake()
        {
            _camera = Camera.main;
            this.UpdateAsObservable().Where(_ => _text != null).Subscribe(_ =>
            {
                // ゲームオブジェクトのワールド位置をスクリーン座標に変換
                Vector3 screenPos = _camera.WorldToScreenPoint(transform.position);

                // Textをキャラに追従
                _text.transform.position = screenPos + (Vector3)_nameDelta;
            });
        }
        private void OnDestroy()
        {
            if (_sequence != null) _sequence.Kill();
            if (_text != null) GraphicalManager.instance.ReleaceText(_text);
        }
    }
}
