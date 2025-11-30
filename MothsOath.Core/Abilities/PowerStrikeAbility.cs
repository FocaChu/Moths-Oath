using MothsOath.Core.Common;
using MothsOath.Core.States;

namespace MothsOath.Core.Abilities;

public class PowerStrikeAbility : IAbility
{
    public string Id => "ability_power_strike";

    public void Execute(Character source, Character target, CombatState gameState)
    {
        int damage = source.BaseStrength * 2;
        target.TakeDamage(damage, false);
    }
}
