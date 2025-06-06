using System.Collections.Generic;
using RPG_003.Battle.Characters;
using UnityEngine;

namespace RPG_003.Battle
{
    /// <summary>
    /// キャラクターの配置管理と、インデックス→CharacterPosition の変換を担当するクラス
    /// </summary>
    public class PositionManager
    {
        // BattleManager から受け渡されるべき変数
        private Dictionary<CharacterPosition, CharacterBase> _characterPositions;

        public PositionManager(Dictionary<CharacterPosition, CharacterBase> characterPositions)
        {
            _characterPositions = characterPositions;
        }

        /// <summary>
        /// Dictionary に登録する／取り除くなどの処理は BattleManager に委譲しつつ、
        /// 「空きポジションを探す」「現在のマップを取得する」あたりを担当
        /// </summary>
        public bool TryGetUsablePosition(out CharacterPosition position, int option = 0)
        {
            int start = option == 2 ? 4 : 0;
            int end = option == 1 ? 3 : 8;
            for (int i = start; i <= end; i++)
            {
                var p = ConvertIndexToPosition(i);
                if (!_characterPositions.ContainsKey(p))
                {
                    position = p;
                    return true;
                }
            }
            position = CharacterPosition.None;
            return false;
        }

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

        public Dictionary<CharacterPosition, CharacterBase> GetCharacterMap()
        {
            return _characterPositions;
        }

        public float GetDepth()
        {
            return 0.5f;
        }

        public CharacterPosition ConvertIndexToPosition(int i)
        {
            return i switch
            {
                0 => CharacterPosition.Player_1,
                1 => CharacterPosition.Player_2,
                2 => CharacterPosition.Player_3,
                3 => CharacterPosition.Player_4,
                4 => CharacterPosition.Enemy_1,
                5 => CharacterPosition.Enemy_2,
                6 => CharacterPosition.Enemy_3,
                7 => CharacterPosition.Enemy_4,
                8 => CharacterPosition.Enemy_5,
                _ => CharacterPosition.None,
            };
        }

        /// <summary>
        /// BattleManager.RemoveCharacter から呼び出す想定。  
        /// _characterPositions から消すだけだから BattleManager でもできるけど、
        /// あえて「位置管理は PositionManager に寄せる」イメージ
        /// </summary>
        public void RemovePosition(CharacterBase character)
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
    }
}
