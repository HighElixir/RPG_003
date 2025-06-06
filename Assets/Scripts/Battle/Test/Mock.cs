using RPG_003.Battle.Characters;
using RPG_003.Battle.Characters.Player;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace RPG_003.Battle
{
    public class Mock : SerializedMonoBehaviour
    {
        [SerializeField] private BattleManager _battleManager;
        [SerializeField] private List<PlayerData> _characterData;
        [SerializeField] private SpawningTable _table;
        private void Start()
        {
            _battleManager.StartBattle(_characterData, _table);
        }
    }
}