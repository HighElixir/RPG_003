using RPG_001.Battle.Characters;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace RPG_001.Battle
{

    public class Positioning : SerializedMonoBehaviour
    {
        // Inspectorに表示するリスト
        [SerializeField]
        private Dictionary<CharacterPosition, Vector2> _entries = new();

        public void SetPosition(CharacterBase target, CharacterPosition position)
        {
            if (!_entries.TryGetValue(position, out var p))
            {
                Debug.LogError($"Position {position} の座標が登録されてないよ！entriesをチェックしてね");
                return;
            }
            target.transform.position = p;
        }
    }
}
