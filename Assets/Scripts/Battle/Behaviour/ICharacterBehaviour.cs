using RPG_003.Battle.Characters;
using System.Collections;

namespace RPG_003.Battle.Behaviour
{
    public interface ICharacterBehaviour
    {
        void Initialize(ICharacter parent, IBattleManager battleManager);
        IEnumerator TurnBehaviour(bool instant = false);

        void OnDeath(ICharacter character)
        {
            // Default implementation can be empty or can log the death event
            // This method can be overridden in derived classes if specific behavior is needed on death
        }
    }
}