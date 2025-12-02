using MothsOath.Core.Abilities;
using MothsOath.Core.Behaviors;
using MothsOath.Core.Common;
using MothsOath.Core.States;

namespace MothsOath.Core.Entities;

public class Enemy : Character
{
    public IBehavior NormalBehavior { get; set; } = null!;

    public IBehavior SpecialBehavior { get; set; } = null!;

    public IAction BasicAttack { get; set; } = null!;

    public IAction SpecialAbility { get; set; } = null!;

    public int SpecialAbilityCooldown { get; set; }

    public int CurrentCooldown { get; set; }


    public List<Character> GetTargets(CombatState gameState)
    {
        if(CurrentCooldown <= 0)
            return SpecialBehavior.GetTargets(this, gameState);
        
        return NormalBehavior.GetTargets(this, gameState);
    }

    public void TakeTurn(CombatState gameState)
    {
        var target = GetTargets(gameState);

        var context = new ActionContext(this, target, gameState, null);

#nullable disable
        if (CurrentCooldown <= 0)
        {
            SpecialAbility.Execute(context);
            CurrentCooldown = SpecialAbilityCooldown;
            Console.WriteLine($"{Name} usou {SpecialAbility.Id}!");
        }
        else
        {
            BasicAttack.Execute(context);
            Console.WriteLine($"{Name} usou {BasicAttack.Id}!");
        }
#nullable disable

        CurrentCooldown--;

        Console.WriteLine($"{Name} terminou seu turno.");
    }
}
