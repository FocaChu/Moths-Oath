using MothsOath.Core.Common;
using MothsOath.Core.Common.EffectInterfaces.Healing;
using MothsOath.Core.Common.EffectInterfaces.StatusEffect;
using MothsOath.Core.Common.Plans;

namespace MothsOath.Core.Abilities;

public abstract class BaseAction
{
    public abstract string Id { get; }

    public abstract void Execute(ActionContext context);

    public virtual bool ValidateTargets(ActionContext context)
    {
        return context.FinalTargets.Count > 0 || context.FinalTargets != null;
    }

    public virtual HealthModifierPlan CalculateCriticalValue(ActionContext context, HealthModifierPlan plan)
    {
        var source = context.Source;
        var criticalChance = source.Stats.TotalCriticalChance;
        var criticalDamageMultiplier = source.Stats.TotalCriticalDamageMultiplier;

        if (criticalChance <= 0)
            return plan;

        int criticalRoll = new Random().Next(0, 100);
        if (criticalRoll < criticalChance)
        {
            plan.FinalValue = (int)(plan.BaseHealthAmount * criticalDamageMultiplier);
            Console.WriteLine($"DEBUG: Critico: {plan.FinalValue}");
        }

        return plan;
    }

    public virtual HealthModifierPlan ApplyDamageModifiers(ActionContext context, HealthModifierPlan plan)
    {
        var damageModifiers = context.Source.StatusEffects.OfType<IOutgoingHealthModifierReactor>().ToList()
            .Concat(context.Source.PassiveEffects.OfType<IOutgoingHealthModifierReactor>().ToList())
            .OrderByDescending(m => m.Priority);

        foreach (var effect in damageModifiers)
        {
            effect.ModifyOutgoingHealthModifier(context, plan);
        }

        return plan;
    }

    public virtual bool ValidateDamagePlan(ActionContext context, HealthModifierPlan plan)
    {
        return context.CanProceed && plan.FinalValue > 0;
    }

    public virtual HealthModifierPlan ApplyHealModifiers(ActionContext context, HealthModifierPlan plan)
    {
        var healModifiers = context.Source.StatusEffects.OfType<IOutgoingHealthModifierReactor>().ToList()
            .Concat(context.Source.PassiveEffects.OfType<IOutgoingHealthModifierReactor>().ToList())
            .OrderByDescending(m => m.Priority);

        foreach (var effect in healModifiers)
        {
            effect.ModifyOutgoingHealthModifier(context, plan);
        }

        return plan;
    }

    public virtual bool ValidateHealPlan(ActionContext context, HealthModifierPlan plan)
    {
        return context.CanProceed && plan.FinalValue > 0;
    }

    public virtual StatusEffectPlan ApplyStatusEffectModifiers(ActionContext context, StatusEffectPlan plan)
    {
        var statusEffectModifiers = context.Source.StatusEffects.OfType<IOutgoingStatusEffectModifier>().ToList()
            .Concat(context.Source.PassiveEffects.OfType<IOutgoingStatusEffectModifier>().ToList())
            .OrderByDescending(m => m.Priority);

        foreach (var effect in statusEffectModifiers)
        {
            effect.ModifyOutgoingStatusEffect(context, plan);
        }

        return plan;
    }

    public virtual bool ValidateStatusEffectPlan(ActionContext context, StatusEffectPlan plan)
    {
        return context.CanProceed && plan.StatusEffect != null && plan.StatusEffect.IsActive();
    }
}
