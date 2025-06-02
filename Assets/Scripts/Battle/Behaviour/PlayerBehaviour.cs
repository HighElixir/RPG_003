using RPG_001.Battle.Characters;
using System.Collections;
using UnityEngine;

namespace RPG_001.Battle.Behaviour
{
    public class PlayerBehaviour : ICharacterBehaviour
    {
        private ICharacter _parent;
        public void Initialize(ICharacter parent)
        {
            _parent = parent;
            parent.OnDeath += OnDeath;
        }

        public IEnumerator TurnBehaviour(bool instant = false)
        {
            yield return new WaitForSeconds(3f); // Simulate a delay for the turn behaviour
        }

        public void OnDeath(ICharacter character)
        {
            // Handle player-specific death logic here
            Debug.Log($"{character.Data.Name} has died in battle.");
            // Additional logic can be added here, such as updating UI or triggering events
        }
    }
}