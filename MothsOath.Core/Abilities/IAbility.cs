using MothsOath.Core.Common;

namespace MothsOath.Core.Abilities;

public interface IAbility
{
    string Id { get; }

    void Execute(Character source, Character target, GameStateManager gameState);
}
