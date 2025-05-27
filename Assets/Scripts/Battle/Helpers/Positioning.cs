using System.Collections.Generic;
using RPG_001.Battle.Characters;
using UnityEngine;

namespace RPG_001.Battle
{
    // Editorで一覧表示できるキー＋値の組み
    [System.Serializable]
    public struct PositionEntry
    {
        public CharacterPosition position;  // キー
        public Vector2 coordinates;        // 値
    }

    public class Positioning : MonoBehaviour
    {
        // Inspectorに表示するリスト
        [SerializeField]
        private List<PositionEntry> entries = new List<PositionEntry>();

        // ランタイム用辞書
        private Dictionary<CharacterPosition, Vector2> _attributes
            = new Dictionary<CharacterPosition, Vector2>();

        // Inspectorで変更があったときにも辞書を更新
        private void OnValidate()
        {
            BuildDictionary();
        }

        // ゲーム起動時にも辞書を構築
        private void Awake()
        {
            BuildDictionary();
        }

        private void BuildDictionary()
        {
            _attributes.Clear();
            foreach (var e in entries)
            {
                // 同じキーが重複しないよう上書き
                _attributes[e.position] = e.coordinates;
            }
        }

        public void SetPosition(CharacterBase target, CharacterPosition position)
        {
            if (!_attributes.TryGetValue(position, out var p))
            {
                Debug.LogError($"Position {position} の座標が登録されてないよ！entriesをチェックしてね");
                return;
            }
            target.transform.position = p;
        }
    }
}
