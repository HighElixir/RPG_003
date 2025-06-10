using RPG_003.Battle.Characters;
using RPG_003.Battle.Factions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPG_003.Battle
{
    public class TargetSelectHelper
    {
        private BattleManager _battleManager;


        public CharacterBase SelectRandomTarget(Faction faction)
        {
            var c = SelectTargetsByType(faction);
            return c.Count > 0 ? c[Random.Range(0, c.Count)] : null;
        }

        public List<CharacterBase> SelectTargetsByType(Faction faction)
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

        public CharacterBase GetCharacter(CharacterPosition position)
        {
            if (_battleManager.GetCharacterMap().ContainsKey(position))
                return _battleManager.GetCharacterMap()[position];
            return null;
        }

        public bool TryGetCharacter(CharacterPosition position, out CharacterBase character)
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
        public CharacterPosition GetPosition(CharacterBase character)
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