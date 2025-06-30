using HighElixir.Pool;
using Sirenix.OdinInspector;
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using RPG_003.Effect;
using UnityEngine.UI;

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
        private bool _isInitialized = false;
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
                btn.transform.SetParent(_container, false);
                btn.onClick.AddListener(
                    () =>
                {
                    OnComplete?.Invoke(data);
                });
            }
        }
        private void Init()
        {
            _buttonPool = new Pool<Button>(_prefab, 100, _container, true);
            _isInitialized = true;
        }
        private void Awake()
        {
            if (!_isInitialized) Init();
        }
    }
}
