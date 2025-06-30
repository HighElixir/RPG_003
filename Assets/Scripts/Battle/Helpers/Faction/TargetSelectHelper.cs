using RPG_003.Battle.Factions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPG_003.Battle
{
    public class TargetSelectHelper
    {
        private BattleManager _battleManager;


        public CharacterObject SelectRandomTarget(Faction faction)
        {
            var c = TargetSelectorsByType(faction);
            return c.Count > 0 ? c[Random.Range(0, c.Count)] : null;
        }

        public List<CharacterObject> SelectRandomTargets(Faction faction, int count, bool canSelectSameTarget)
        {
            var candidates = TargetSelectorsByType(faction);
            var targets = new List<CharacterObject>();

            // 重複禁止で要望数が候補数より多い場合は上限を候補数に合わせる
            if (!canSelectSameTarget && count > candidates.Count)
                count = candidates.Count;

            // 必要な数だけランダム取得
            while (targets.Count < count)
            {
                var idx = Random.Range(0, candidates.Count);
                var pick = candidates[idx];

                // 重複許可 or 未登録なら追加
                if (canSelectSameTarget || !targets.Contains(pick))
                {
                    targets.Add(pick);
                }
                // 重複禁止で満たせないならループ抜ける（安全策）
                else if (!canSelectSameTarget && targets.Count >= candidates.Count)
                {
                    break;
                }
            }

            return targets;
        }


        public List<CharacterObject> TargetSelectorsByType(Faction faction)
        {
            var characters = _battleManager.GetCharacterMap();
            return faction switch
            {
                Faction.All => characters.Values.ToList(),
                Faction.Enemy => characters.Values.Where(c => c.IsEnemy()).ToList(),
                Faction.Ally => characters.Values.Where(c => c.IsAlly()).ToList(),
                _ => null
            };
        }

        public CharacterObject GetCharacter(CharacterPosition position)
        {
            if (_battleManager.GetCharacterMap().ContainsKey(position))
                return _battleManager.GetCharacterMap()[position];
            return null;
        }

        public bool TryGetCharacter(CharacterPosition position, out CharacterObject character)
        {
            var map = _battleManager.GetCharacterMap();
            if (map.ContainsKey(position))
            {
                character = map[position];
                return true;
            }
            character = null;
            return false;
        }
        public CharacterPosition GetPosition(CharacterObject character)
        {
            if (character != null)
            {
                var map = _battleManager.GetCharacterMap();
                foreach (var c in map)
                {
                    if (Equals(character, c.Value))
                    {
                        return c.Key;
                    }
                }
            }
            return CharacterPosition.None;
        }
        public TargetSelectHelper(BattleManager battleManager)
        {
            _battleManager = battleManager;
        }
    }
}