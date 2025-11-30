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
            Console.WriteLine($"{Name} usou {SpecialAbility.Id}!");
        }
        else
        {
            BasicAttack.Execute(this, target, gameState);
            Console.WriteLine($"{Name} usou {BasicAttack.Id}!");
        }
        #nullable disable

        CurrentCooldown--;

        Console.WriteLine($"{Name} terminou seu turno.");
    }
}
