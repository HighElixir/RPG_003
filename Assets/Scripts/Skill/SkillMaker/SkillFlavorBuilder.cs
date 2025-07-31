using HighElixir.Pool;
using Sirenix.OdinInspector;
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UniRx;
using UniRx.Triggers;
using RPG_003.Effect;
using UnityEngine.UI;
using Lean.Gui;

namespace RPG_003.Skills
{
    public class SkillFlavorBuilder : MonoBehaviour
    {
        public class Result
        {
            public SoundVFXData Data { get; set; }
            public Sprite Icon { get; set; }
        }

        [SerializeField, BoxGroup("Pool")] private Pool<Button> _buttonPool;
        [SerializeField, BoxGroup("Pool")] private Button _prefab; 
        [SerializeField, BoxGroup("Pool")] private Transform _container;
        [SerializeField, BoxGroup("AddressablesKey")] private string _folderLabel = "SoundVFX"; // Addressablesのラベルを指定
        [SerializeField, BoxGroup("Reference")] private SkillMaker _maker;
        [SerializeField, BoxGroup("Reference")] private LeanWindow _window;
        [SerializeField, BoxGroup("Reference")] private VerticalLayoutGroup _verticalLayoutGroup;
        private bool _isInitialized = false;
        private bool _enable = false;
        public Action<SoundVFXData> OnComplete { get; private set; }

        // 呼び出し側でラベルを変えたいなら引数で渡せるようにした
        public async UniTask CreateButtonsAsync(string label = null, CancellationToken token = default)
        {
            if (!_isInitialized) Init(); // 初期化されていない場合は初期化
            var key = label ?? _folderLabel;

            // ── 既存のボタンをプールに返却 ──
            foreach (var btn in _buttonPool.InUse)
            {
                btn.onClick.RemoveAllListeners(); // 既存のリスナーを削除
                _buttonPool.Release(btn);
            }

            // 【事実】Addressables.LoadAssetsAsync<T> でラベル配下の全アセットを非同期取得できる  
            var datas = await Addressables
                .LoadAssetsAsync<SoundVFXData>(key, null)
                .ToUniTask(cancellationToken: token);

            // ── ロードが終わったらボタン生成 ──
            foreach (var data in datas)
            {
                var btn = _buttonPool.Get();
                btn.transform.SetParent(_verticalLayoutGroup.transform, false);
                btn.onClick.AddListener(
                    () =>
                {
                    OnComplete?.Invoke(data);
                });
            }
        }
        public void OnClick()
        {
            if (_enable)
            {
                _window.On = true;
            }
        }
        private void Init()
        {
            if (_maker == null)
            {
                Debug.LogError("");
                enabled = false;
                return;
            }
            _buttonPool = new Pool<Button>(_prefab, 20, _container);
            _maker.Current.Subscribe(res => 
            {
                if (res != SkillMaker.SkillType.None) _enable = true;
                else _enable = false;
            }).AddTo(this);
            _isInitialized = true;
        }
        private void Awake()
        {
            if (!_isInitialized) Init();
        }
    }
}
