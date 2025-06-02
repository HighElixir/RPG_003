using RPG_001.Battle.Characters;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace RPG_001.Battle
{
    public class Mock : SerializedMonoBehaviour
    {
        [SerializeField] private BattleManager _battleManager;
        [SerializeField] private List<CharacterData> _characterData;
        [SerializeField] private SpawningTable _table;
        private void Start()
        {
            _battleManager.StartBattle(_characterData, _table);
        }
    }
}