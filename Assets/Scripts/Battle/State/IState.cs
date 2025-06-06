using HighElixir.StateMachine;
using RPG_003.Battle.Characters;

namespace RPG_003.Battle
{
    public interface IState : IState<ICharacter>
    {
    }
}