using MothsOath.Core.Abilities;
using MothsOath.Core.Behaviors;
using MothsOath.Core.Common;
using MothsOath.Core.Common.EffectInterfaces;
using MothsOath.Core.States;

namespace MothsOath.Core.Entities;

public class Enemy : Character
{
    public string BiomeId { get; set; } = "forest_biome";

    public IBehavior NormalBehavior { get; set; } = null!;

    public IBehavior SpecialBehavior { get; set; } = null!;

    public BaseAction BasicAttack { get; set; } = null!;

    public BaseAction SpecialAbility { get; set; } = null!;

    public int SpecialAbilityCooldown { get; set; }

    public int CurrentCooldown { get; set; }

    public bool CanUseSpecial => CurrentCooldown <= 0;


    public List<Character> GetTargets(CombatState gameState)
    {
        if (this.CanUseSpecial)
            return SpecialBehavior.GetTargets(this, gameState);

        return NormalBehavior.GetTargets(this, gameState);
    }

    public ActionContext CreateActionContext(CombatState gameState)
    {
        var context = new ActionContext(this, GetTargets(gameState), gameState, null);
        context.CanUseSpecial = this.CanUseSpecial;

        var actionModifiers = this.StatusEffects.OfType<IActionPlanModifier>().ToList();

        foreach (var effect in actionModifiers)
        {
            ((IActionPlanModifier)effect).ModifyActionPlan(context);
        }

        return context;
    }

    public void TakeTurn(CombatState gameState)
    {
        var context = CreateActionContext(gameState);

        if (!context.CanProceed || context.FinalTargets == null || context.FinalTargets.Count == 0) return;

        if (context.CanUseSpecial)
        {
            SpecialAbility.Execute(context);
            Console.WriteLine($"{Name} usou {SpecialAbility.Id}!");
        }
        else
        {
            BasicAttack.Execute(context);
            Console.WriteLine($"{Name} usou {BasicAttack.Id}!");
        }

        if (this.CanUseSpecial)
        {
            CurrentCooldown = SpecialAbilityCooldown;
        }
        else
        {
            CurrentCooldown--;
        }

        Console.WriteLine($"{Name} terminou seu turno.");
    }
}
