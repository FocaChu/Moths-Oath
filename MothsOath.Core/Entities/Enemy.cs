using MothsOath.Core.Abilities;
using MothsOath.Core.Behaviors;
using MothsOath.Core.Common;
using MothsOath.Core.States;
using MothsOath.Core.StatusEffect;
using MothsOath.Core.StatusEffect.Interfaces;

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

    public ActionPlan CreateActionPlan(CombatState gameState)
    {
        var plan = new ActionPlan
        {
            Source = this,
            BaseTargets = GetTargets(gameState)
        };

            plan.CanUseSpecial = CurrentCooldown <= 0;

            var intentModifiers = System.Linq.Enumerable.ToList(
                System.Linq.Enumerable.Where(
                    System.Linq.Enumerable.OfType<BaseStatusEffect>(this.StatusEffects ?? System.Linq.Enumerable.Empty<BaseStatusEffect>()),
                    e => e is IActionPlanModifier
                )
            );

            foreach (var effect in intentModifiers)
            {
                ((IActionPlanModifier)effect).ModifyIntent(plan, gameState);
            }

        return plan;
    }

    public void TakeTurn(CombatState gameState)
    {
        var plan = CreateActionPlan(gameState);


        var context = new ActionContext(this, plan.FinalTargets, gameState, null);

#nullable disable
        if (!plan.CanProceed || plan.FinalTargets.Count == 0) return;

        if (CurrentCooldown <= 0 && plan.CanUseSpecial)
        {
            SpecialAbility.Execute(context);
            CurrentCooldown = SpecialAbilityCooldown;
            Console.WriteLine($"{Name} usou {SpecialAbility.Id}!");
        }
        else if(CurrentCooldown <= 0 && !plan.CanUseSpecial)
        {
            CurrentCooldown = SpecialAbilityCooldown;
            BasicAttack.Execute(context);
            Console.WriteLine($"{Name} usou {BasicAttack.Id}!");
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
