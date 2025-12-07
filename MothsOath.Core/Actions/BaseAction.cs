using MothsOath.Core.Common;
using MothsOath.Core.Common.EffectInterfaces.Damage;
using MothsOath.Core.Common.EffectInterfaces.Healing;
using MothsOath.Core.Common.EffectInterfaces.StatusEffect;
using MothsOath.Core.Common.Plans;

namespace MothsOath.Core.Abilities;

public abstract class BaseAction
{
    public abstract string Id { get; }

    public abstract void Execute(ActionContext context);

    public virtual bool ValidadeTargets(ActionContext context)
    {
        return context.FinalTargets.Count > 0 || context.FinalTargets != null;
    }

    public virtual DamagePlan ApplyDamageModifiers(ActionContext context, DamagePlan plan)
    {
        var damageModifiers = context.Source.StatusEffects.OfType<IOutgoingDamageModifier>().ToList();
        foreach (var effect in damageModifiers)
        {
            effect.ModifyOutgoingDamage(context, plan);
        }
        return plan;
    }

    public virtual bool ValidateDamagePlan(ActionContext context, DamagePlan plan)
    {
        return context.CanProceed && plan.FinalDamageAmount > 0;
    }

    public virtual HealPlan ApplyHealModifiers(ActionContext context, HealPlan plan)
    {
        var healModifiers = context.Source.StatusEffects.OfType<IOutgoingHealingModifier>().ToList();
        foreach (var effect in healModifiers)
        {
            effect.ModifyOutgoingHealing(context, plan);
        }
        return plan;
    }

    public virtual bool ValidateHealPlan(ActionContext context, HealPlan plan)
    {
        return context.CanProceed && plan.FinalHealAmount > 0;
    }

    public virtual StatusEffectPlan ApplyStatusEffectModifiers(ActionContext context, StatusEffectPlan plan)
    {
        var statusEffectModifiers = context.Source.StatusEffects.OfType<IOutgoingStatusEffectModifier>().ToList();
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
