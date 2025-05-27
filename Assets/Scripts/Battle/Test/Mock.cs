using RPG_001.Characters;
using System.Collections.Generic;
using UnityEngine;

namespace RPG_001.Battle
{
    public class Mock : MonoBehaviour
    {
        [SerializeField] private BattleManager _battleManager;
        [SerializeField] private List<CharacterData> _characterData;

        private void Start()
        {
            _battleManager.StartBattle(_characterData);
        }
    }
}