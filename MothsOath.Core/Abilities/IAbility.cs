using MothsOath.Core.Common;
using MothsOath.Core.States;

namespace MothsOath.Core.Abilities;

public interface IAbility
{
    string Id { get; }

    void Execute(Character source, Character target, CombatState gameState);
}
