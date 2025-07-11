using HighElixir;
using RPG_003.Battle.Factions;
using System.Collections.Generic;
using UnityEngine;

namespace RPG_003.Battle
{
    /// <summary>
    /// キャラクターの配置管理を担当するクラス
    /// </summary>
    public class PositionManager
    {
        private Dictionary<CharacterPosition, Unit> _characterPositions = new();

        /// <summary>
        /// CharacterPositionの中から使われていない枠を探す
        /// </summary>
        /// <param name="position">結果</param>
        /// <returns>発見に成功したかどうか</returns>
        public bool TryGetUsablePosition(out CharacterPosition position, Faction faction)
        {
            int start = faction == Faction.Enemy ? 4 : 0;
            int end = faction == Faction.Ally ? 3 : 8;
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

        // 全てのキャラをリストにして返す
        public List<Unit> GetCharacters()
        {
            return new List<Unit>(_characterPositions.Values);
        }
        public IReadOnlyDictionary<CharacterPosition, Unit> GetCharacterMap()
        {
            return _characterPositions;
        }
        public void RemoveCharacter(Unit character)
        {
            if (character == null || !_characterPositions.ContainsValue(character)) return;
            var pos = CharacterPosition.None;
            foreach (var kv in _characterPositions)
            {
                if (kv.Value.Equals(character))
                {
                    pos = kv.Key;
                    break;
                }
            }
            if (pos == CharacterPosition.None) return;
            _characterPositions.Remove(pos);
        }
        public void RegisterCharacter(CharacterPosition position, Unit character)
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

        /// <summary>
        /// 各派閥の登録済みのキャラクターを返す
        /// </summary>
        /// <param name="faction">検索対象</param>
        /// <param name="needAlive">生存している必要があるかどうか</param>
        /// <returns></returns>
        public int FactionCount(Faction faction, bool needAlive = true)
        {
            int res = 0;
            foreach (var p in EnumWrapper.GetEnumList<CharacterPosition>())
            {
                if (p.IsSameFaction(faction) && _characterPositions.ContainsKey(p) && (!needAlive || _characterPositions[p].IsAlive))
                    res++;
            }
            return res;
        }

        public (int allies, int enemies) AllFactionCount(bool isAlive = true)
        {
            var res = (0, 0);
            res.Item1 = FactionCount(Faction.Ally, isAlive);
            res.Item2 = FactionCount(Faction.Enemy, isAlive);
            return res;
        }
    }
}
