using RPG_003.Battle.Characters;
using RPG_003.Battle.Characters.Player;
using RPG_003.Battle.Skills;
using RPG_003.Core;
using RPG_003.Skills;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace RPG_003.Battle
{
    public class Mock : SerializedMonoBehaviour
    {
        [SerializeField] private BattleManager _battleManager;
        [SerializeField] private SpawningTable _table;
        [SerializeField] private SpriteRenderer _backGround;

        [SerializeField] private bool _useDataHolder = true;
        [SerializeField] private List<PlayerData> _players;
        private void Start()
        {
            if (_useDataHolder)
                _players = new(GameDataHolder.instance.GetPlayerDatas());
                GraphicalManager.instance.SetBackground(_backGround);
            _battleManager.StartBattle(_players, _table);
        }
    }
}