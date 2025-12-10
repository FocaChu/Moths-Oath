using MothsOath.Core.Common;
using MothsOath.Core.Common.Plans;
using MothsOath.Core.Models.Enums;
using MothsOath.Core.StatusEffect.ConcreteEffects;

namespace MothsOath.Core.Abilities;

public class ToxicJabAction : BaseAction
{
    public override string Id => "action_toxic_jab";

    public override void Execute(ActionContext context)
    {
        int damage = context.Source.Stats.TotalStrength;

        var damagePlan = new HealthModifierPlan(damage, HealthModifierType.Damage);

        if (context.CanOutgoingModifiers)
            ApplyDamageModifiers(context, damagePlan);

        if (!ValidateTargets(context) || !ValidateDamagePlan(context, damagePlan))
            return;

        if (damagePlan.CanCritical)
            damagePlan = CalculateCriticalValue(context, damagePlan);

        var rng = new Random();
        var target = context.FinalTargets[rng.Next(context.FinalTargets.Count)];

        context.FinalTargets.Clear();
        context.FinalTargets.Add(target);

        target.HandleHealthModifier(context, damagePlan);

        var poisonEffect = new PoisonEffect(level: (int)(context.Source.Stats.BaseKnowledge / 2), duration: 3);
        var effectPlan = new StatusEffectPlan(poisonEffect);

        if (context.CanOutgoingModifiers)
            ApplyStatusEffectModifiers(context, effectPlan);

        if (!ValidateTargets(context) || !ValidateStatusEffectPlan(context, effectPlan))
            return;

        target.ApplyStatusEffect(context, effectPlan);
    }
}
