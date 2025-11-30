using MothsOath.Core.Abilities;
using MothsOath.Core.Common;
using MothsOath.Core.States;

namespace MothsOath.Core.Entities;

public class Enemy : Character
{
    public IAbility BasicAttack { get; set; } = null!;

    public IAbility SpecialAbility { get; set; } = null!;

    public int SpecialAbilityCooldown { get; set; }

    public int CurrentCooldown { get; set; }


    public void TakeTurn(CombatState gameState)
    {
        var target = gameState.Player;

        #nullable disable
        if (CurrentCooldown <= 0)
        {
            SpecialAbility.Execute(this, target, gameState);
            CurrentCooldown = SpecialAbilityCooldown;
        }
        else
        {
            BasicAttack.Execute(this, target, gameState);
        }
        #nullable disable

        CurrentCooldown--;

        Console.WriteLine($"{Name} terminou seu turno.");
    }
}
