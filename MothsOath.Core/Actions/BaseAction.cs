using MothsOath.Core.Common;
using MothsOath.Core.Common.EffectInterfaces;
using MothsOath.Core.Common.Plans;

namespace MothsOath.Core.Abilities;

public abstract class BaseAction
{
    public abstract string Id { get; }

    public abstract void Execute(ActionContext context);

    public virtual bool CheckTargets(ActionContext context)
    {
        return context.FinalTargets.Count == 0;
    }

    public virtual DamagePlan CreateDamagePlan(ActionContext context, int baseDamage)
    {
        var plan = new DamagePlan(
            baseDamage,
            false,
            true
        );

        plan = ApplyDamageModifiers(context, plan);

        return plan;
    }

    public virtual HealPlan CreateHealPlan(ActionContext context, int baseHeal)
    {
        var plan = new HealPlan(
            baseHeal,
            true
        );

        plan = ApplyHealModifiers(context, plan);

        return plan;
    }

    private DamagePlan ApplyDamageModifiers(ActionContext context, DamagePlan plan)
    {
        var damageModifiers = context.Source.StatusEffects.OfType<IOutgoingDamageModifier>().ToList();
        foreach (var effect in damageModifiers)
        {
            effect.ModifyOutgoingDamage(context, plan);
        }
        return plan;
    }

    private HealPlan ApplyHealModifiers(ActionContext context, HealPlan plan)
    {
        var healModifiers = context.Source.StatusEffects.OfType<IOutgoingHealingModifier>().ToList();
        foreach (var effect in healModifiers)
        {
            effect.ModifyOutgoingHealing(context, plan);
        }
        return plan;
    }
}
