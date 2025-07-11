using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HighElixir.UI
{
    // スライド方向を指定する列挙
    public enum MoveDirection { Left, Right, Up, Down }

    public class OpenableUI : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private RectTransform _open;
        [SerializeField] private Canvas _canvas;
        [SerializeField] private MoveDirection _direction;
        [SerializeField] private float _duration = 0.3f;
        [SerializeField] private bool _hideInStart = true;

        private bool _opened = false;
        private Vector3 _closedPosition;
        private Tween _tween;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;
            Move();
        }

        private void Move()
        {
            if (_tween != null && _tween.IsPlaying()) return;

            // 開閉対象の位置を決定：開いていなければ_closedPosition(開いた位置)へ、開いていれば隠す位置へ
            Vector3 target = _opened ? GetHiddenPosition() : _closedPosition;

            _tween = _open.DOLocalMove(target, _duration)
                         .SetEase(Ease.OutCubic)
                         .OnComplete(() =>
                         {
                             _opened = !_opened;
                             if (_opened)
                                 ExecuteEvents.Execute<IOpendEvent>(gameObject, null, (h, d) => h.Opened());
                             else
                                 ExecuteEvents.Execute<IOpendEvent>(gameObject, null, (h, d) => h.Closed());
                         });
        }

        // 隠す位置をCanvas外に計算
        private Vector3 GetHiddenPosition()
        {
            Rect rect = _canvas.pixelRect;
            var pos = _open.localPosition;
            float w = _open.rect.width;
            float h = _open.rect.height;

            return _direction switch
            {
                MoveDirection.Left => new Vector3(-(rect.width / 2 + w), pos.y, pos.z),
                MoveDirection.Right => new Vector3((rect.width / 2 + w), pos.y, pos.z),
                MoveDirection.Up => new Vector3(pos.x, (rect.height / 2 + h), pos.z),
                MoveDirection.Down => new Vector3(pos.x, -(rect.height / 2 + h), pos.z),
                _ => pos,
            };
        }

        private void Awake()
        {
            // 初期位置(開いたときの位置)を計算
            Vector3 initial = _open.localPosition;
            Rect rect = _canvas.pixelRect;
            float w = _open.rect.width;
            float h = _open.rect.height;

            switch (_direction)
            {
                case MoveDirection.Left:
                    // 右端に合わせる
                    _closedPosition = new Vector3(rect.width / 2 - w / 2, initial.y, initial.z);
                    break;
                case MoveDirection.Right:
                    // 左端に合わせる
                    _closedPosition = new Vector3(-rect.width / 2 + w / 2, initial.y, initial.z);
                    break;
                case MoveDirection.Up:
                    // 下端に合わせる
                    _closedPosition = new Vector3(initial.x, -rect.height / 2 + h / 2, initial.z);
                    break;
                case MoveDirection.Down:
                    // 上端に合わせる
                    _closedPosition = new Vector3(initial.x, rect.height / 2 - h / 2, initial.z);
                    break;
                default:
                    _closedPosition = initial;
                    break;
            }

            if (_hideInStart)
            {
                // 起動時に隠す
                _open.localPosition = GetHiddenPosition();
                _opened = false;
            }
        }
    }

    // 開閉完了時に呼び出すイベント用インターフェイス
    public interface IOpendEvent : IEventSystemHandler
    {
        void Opened();
        void Closed();
    }
}
