using MothsOath.Core.Factories;
using MothsOath.Core.States;

namespace MothsOath.Core;

public class GameManager
{
    private readonly StateFactory _stateFactory;
    public IGameState CurrentState { get; private set; }
    public event Action<IGameState> OnStateChanged;

    public GameManager(StateFactory stateFactory)
    {
        _stateFactory = stateFactory;
        CurrentState = _stateFactory.CreateMainMenuState(this);
    }

    public void TransitionToState(IGameState newState)
    {
        CurrentState.OnExit(); 
        CurrentState = newState;
        CurrentState.OnEnter(); 
        OnStateChanged?.Invoke(newState);
    }
}
