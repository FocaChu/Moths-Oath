using MothsOath.Core.Factories;
using MothsOath.Core.States;

namespace MothsOath.Core;

public class GameStateManager
{
    public StateFactory StateFactory;

    public string Biome { get; set; } = "forest_biome";

    public IGameState CurrentState { get; private set; } 


    public event Action<IGameState> OnStateChanged;

    public GameStateManager(StateFactory stateFactory)
    {
        StateFactory = stateFactory;
        CurrentState = StateFactory.CreateMainMenuState(this);
    }

    public void TransitionToState(IGameState newState)
    {
        CurrentState.OnExit(); 
        CurrentState = newState;
        CurrentState.OnEnter(); 
        OnStateChanged?.Invoke(newState);
    }
}
