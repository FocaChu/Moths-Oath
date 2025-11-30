using MothsOath.Core.Common;

namespace MothsOath.Core.Abilities;

public class BasicAttackAbility : IAbility
{
    public string Id => "ability_basic_attack";

    public void Execute(Character source, Character target, GameStateManager gameState)
    {
        int damage = source.BaseStrength;
        target.TakeDamage(damage, false);
    }
}
