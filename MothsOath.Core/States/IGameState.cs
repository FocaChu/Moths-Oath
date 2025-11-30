namespace MothsOath.Core.States;

public interface IGameState
{
    void OnEnter();

    void OnExit();

    void Update();
}