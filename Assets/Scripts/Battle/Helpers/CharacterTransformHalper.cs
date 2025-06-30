using HighElixir;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections.Generic;
using UnityEngine;

namespace RPG_003.Battle
{

    public class CharacterTransformHelper : SerializedMonoBehaviour
    {
        [OdinSerialize]
        [DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.Foldout)]
        public Dictionary<CharacterPosition, Vector2> _entries = new();

        /// <summary>
        /// 指定されたポジションの座標を取得してオブジェクトを移動する
        /// </summary>
        public void SetPosition(CharacterObject target, CharacterPosition position)
        {
            if (!_entries.TryGetValue(position, out var p))
            {
                Debug.LogError($"Position {position} の座標が登録されてないよ！entriesをチェックしてね");
                return;
            }

            target.transform.position = new Vector3(p.x, p.y, target.transform.position.z);
        }

        public Vector2 GetPosition(CharacterPosition position)
        {
            return _entries.TryGetValue(position, out var p) ? p : Vector2.zero;
        }

        [Button("Set")]
        private void Set()
        {
            _entries.Clear();
            foreach (var e in EnumWrapper.GetEnumList<CharacterPosition>())
            {
                if (e == CharacterPosition.None) continue;
                _entries[e] = Vector2.zero;
            }
        }
    }
}
