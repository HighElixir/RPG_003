using RPG_001.Battle.Characters;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace RPG_001.Battle
{
    public class TargetSelecter
    {
        private IBattleManager _battleManager;

        public IBattleManager BattleManager
        {
            get => _battleManager;
            set => _battleManager = value;
        }

        /// <param name="option">0=全て,1=味方,2=敵</param>
        public CharacterBase SelectRandomTarget(int option)
        {
            var c = SelectTargetsByType(option);
            return c.Count > 0 ? c[Random.Range(0, c.Count)] : null;
        }

        public List<CharacterBase> SelectTargetsByType(int option = 0)
        {
            var characters = _battleManager.GetCharacterMap();
            if (option == 0) // 全て
            {
                return characters.Values.ToList();
            }
            else if (option == 1) // 味方
            {
                return characters.Values.Where(c => IsAlly(c)).ToList();
            }
            else if (option == 2) // 敵
            {
                return characters.Values.Where(c => IsEnemy(c)).ToList();
            }
            return new List<CharacterBase>();
        }

        public TargetSelecter(IBattleManager battleManager)
        {
            _battleManager = battleManager;
        }

        private bool IsAlly(ICharacter character)
        {
            return
                character.Position == CharacterPosition.Player_1 ||
                character.Position == CharacterPosition.Player_2 ||
                character.Position == CharacterPosition.Player_3 ||
                character.Position == CharacterPosition.Player_4;
        }

        private bool IsEnemy(ICharacter character)
        {
            return
                character.Position == CharacterPosition.Enemy_1 ||
                character.Position == CharacterPosition.Enemy_2 ||
                character.Position == CharacterPosition.Enemy_3 ||
                character.Position == CharacterPosition.Enemy_4;
        }
    }
}