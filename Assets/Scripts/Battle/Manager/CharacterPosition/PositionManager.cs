using RPG_003.Battle.Characters;
using System.Collections.Generic;
using UnityEngine;

namespace RPG_003.Battle
{
    /// <summary>
    /// キャラクターの配置管理を担当するクラス
    /// </summary>
    public class PositionManager
    {
        // BattleManager から受け渡されるべき変数
        private Dictionary<CharacterPosition, CharacterBase> _characterPositions = new();

        public PositionManager(out IReadOnlyDictionary<CharacterPosition, CharacterBase> characterPositions)
        {
            _characterPositions = new();
            characterPositions = _characterPositions;
        }

        /// <summary>
        /// CharacterPositionの中から使われていない枠を探す
        /// </summary>
        /// <param name="position">結果</param>
        /// <param name="option">0 = 未指定, 1 = 味方のみ, 2 = 敵</param>
        /// <returns>発見に成功したかどうか</returns>
        public bool TryGetUsablePosition(out CharacterPosition position, int option = 0)
        {
            int start = option == 2 ? 4 : 0;
            int end = option == 1 ? 3 : 8;
            for (int i = start; i <= end; i++)
            {
                var p = (CharacterPosition)i;
                if (!_characterPositions.ContainsKey(p))
                {
                    position = p;
                    return true;
                }
            }
            position = CharacterPosition.None;
            return false;
        }
        /// <summary>
        /// CharacterPositionの中から使われていない枠を探す
        /// </summary>
        /// <param name="option">0 = 未指定, 1 = 味方のみ, 2 = 敵</param>
        public CharacterPosition GetUsablePosition(int option = 0)
        {
            if (TryGetUsablePosition(out var pos, option))
                return pos;
            return CharacterPosition.None;
        }

        public List<CharacterBase> GetCharacters()
        {
            return new List<CharacterBase>(_characterPositions.Values);
        }

        public IReadOnlyDictionary<CharacterPosition, CharacterBase> GetCharacterMap()
        {
            return _characterPositions;
        }

        // 仮置き
        public float GetDepth()
        {
            return 0.5f;
        }

        public void RemoveCharacter(CharacterBase character)
        {
            if (character == null || !_characterPositions.ContainsValue(character)) return;
            var pos = CharacterPosition.None;
            foreach (var kv in _characterPositions)
            {
                if (kv.Value == character)
                {
                    pos = kv.Key;
                    break;
                }
            }
            if (pos == CharacterPosition.None) return;
            _characterPositions.Remove(pos);
        }

        public void RegisterCharacter(CharacterPosition position, CharacterBase character)
        {
            if (_characterPositions.ContainsKey(position))
            {
                Debug.LogError("既に登録されているよ！");
                return;
            }
            _characterPositions.Add(position, character);
            character.Position = position;
        }

        public void Clear()
        {
            _characterPositions.Clear();
        }
    }
}
