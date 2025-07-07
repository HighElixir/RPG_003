using System;
using System.Collections.Generic;
using UnityEngine;
using RPG_003.Character;

namespace RPG_003.Core
{
    /// <summary>
    /// チーム編成を兼ねている
    /// </summary>
    [Serializable]
    public class PlayerDataHolder
    {
        // == ==
        [SerializeField] private List<CharacterDataHolder> _data = new();

        // == Property ==
        public List<CharacterDataHolder> Data => _data;
        public int Count => _data.Count;

        // === Public ===
        public void Add(CharacterDataHolder data)
        {
            _data.Add(data);
        }

        // === Constracter ===
        public PlayerDataHolder() { }
    }
}