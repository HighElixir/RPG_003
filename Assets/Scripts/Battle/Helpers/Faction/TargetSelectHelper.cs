using RPG_003.Battle.Factions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPG_003.Battle
{
    public class TargetSelectHelper
    {
        private BattleManager _battleManager;


        public Unit SelectRandomTarget(Faction faction)
        {
            var c = TargetSelectorsByType(faction);
            return c.Count > 0 ? c[Random.Range(0, c.Count)] : null;
        }
        public List<Unit> SelectRandomTargets(AISkillSet skillSet, bool isReverse)
        {
            var target = skillSet.skill.Target;
            var faction = isReverse ? target.Faction.GetReverse() : target.Faction;
            var count = target.Count;
            var canSelectSameTarget = target.CanSelectSameTarget;
            var brain = skillSet.brain;
            var pow = skillSet.pow;
            if (Random.Range(0f, 1f) <= pow)
                return SelectRandomTargets(faction, count, canSelectSameTarget, brain);
            else
                return SelectRandomTargets(faction, count, canSelectSameTarget);
        }
        public List<Unit> SelectRandomTargets(Faction faction, int count, bool canSelectSameTarget, TargetSource brain = TargetSource.Random)
        {
            var candidates = TargetSelectorsByType(faction);
            var targets = new List<Unit>();

            if (!canSelectSameTarget && count > candidates.Count)
                count = candidates.Count;

            while (targets.Count < count)
            {
                Unit pick;
                switch (brain)
                {
                    case TargetSource.Random:
                        pick = RandomPick(candidates);
                        break;
                    case TargetSource.MinHP:
                        pick = FixedHpPick(candidates, false);
                        break;
                    case TargetSource.MaxHP:
                        pick = FixedHpPick(candidates, true);
                        break;
                    case TargetSource.MinHPRatio:
                        pick = HpRatioPick(candidates, false);
                        break;
                    case TargetSource.MaxHPRatio:
                        pick = HpRatioPick(candidates, true);
                        break;
                    default:
                        pick = RandomPick(candidates);
                        break;
                }

                if (pick == null)
                    break;

                if (canSelectSameTarget || !targets.Contains(pick))
                    targets.Add(pick);
                else if (!canSelectSameTarget && targets.Count >= candidates.Count)
                    break;
            }

            return targets;
        }

        // ランダムピックはそのまま
        private Unit RandomPick(List<Unit> pool) => pool.Count > 0 ? pool[Random.Range(0, pool.Count)] : null;

        // 絶対HPで最小 or 最大を取る
        private Unit FixedHpPick(List<Unit> pool, bool isMax)
        {
            if (pool == null || pool.Count == 0) return null;
            return isMax
                ? pool.OrderByDescending(u => u.StatusManager.HP).First()
                : pool.OrderBy(u => u.StatusManager.HP).First();
        }

        // HP割合（CurrentHP / MaxHP）で最小 or 最大を取る
        private Unit HpRatioPick(List<Unit> pool, bool isMax)
        {
            if (pool == null || pool.Count == 0) return null;
            return isMax
                ? pool.OrderByDescending(u => (float)u.StatusManager.HP / u.StatusManager.MaxHP).First()
                : pool.OrderBy(u => (float)u.StatusManager.HP / u.StatusManager.MaxHP).First();
        }

        public List<Unit> TargetSelectorsByType(Faction faction)
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

        public Unit GetCharacter(CharacterPosition position)
        {
            if (_battleManager.GetCharacterMap().ContainsKey(position))
                return _battleManager.GetCharacterMap()[position];
            return null;
        }

        public bool TryGetCharacter(CharacterPosition position, out Unit character)
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
        public CharacterPosition GetPosition(Unit character)
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