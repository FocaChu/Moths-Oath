using MothsOath.Core.States;

namespace MothsOath.Core;

public class GameManager
{
    public IGameState CurrentState { get; private set; }

    public event Action<IGameState> OnStateChanged;

    public GameManager()
    {
        CurrentState = new MainMenuState(this); 
    }

    public void TransitionToState(IGameState newState)
    {
        CurrentState.OnExit(); 
        CurrentState = newState;
        CurrentState.OnEnter(); 
        OnStateChanged?.Invoke(newState);
    }
}
