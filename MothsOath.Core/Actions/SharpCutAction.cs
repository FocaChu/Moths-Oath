using MothsOath.Core.Common;
using MothsOath.Core.Common.Plans;
using MothsOath.Core.Models.Enums;
using MothsOath.Core.StatusEffect.ConcreteEffects;

namespace MothsOath.Core.Abilities;

public class SharpCutAction : BaseAction
{
    public override string Id => "action_sharp_cut";

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

        target.ReceiveDamage(context, damagePlan);

        var bleedingEffect = new BleedingEffect(level: (int)(context.Source.Stats.BaseStrength / 2) + 1, duration: 2);
        var effectPlan = new StatusEffectPlan(bleedingEffect);

        if (context.CanOutgoingModifiers)
            ApplyStatusEffectModifiers(context, effectPlan);

        if (!ValidateTargets(context) || !ValidateStatusEffectPlan(context, effectPlan))
            return;

        target.ApplyStatusEffect(context, effectPlan);

        Console.WriteLine($"{context.Source.Name} cut {target.Name} leaving they bleeding!");
    }
}
