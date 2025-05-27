using HighElixir.StateMachine;
using RPG_001.Battle.Characters;

namespace RPG_001.Battle
{
    public interface IState : IState<ICharacter>
    {
    }
}