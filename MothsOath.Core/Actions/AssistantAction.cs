using MothsOath.Core.Abilities;
using MothsOath.Core.Common;
using MothsOath.Core.Common.Plans;
using MothsOath.Core.StatusEffect.ConcreteStatusEffects;

namespace MothsOath.Core.Actions;

public class AssistantAction : BaseAction
{
    public override string Id => "action_assistant";

    public override void Execute(ActionContext context)
    {
        if (!ValidateTargets(context))
            return;

        var rng = new Random();
        var target = context.FinalTargets[rng.Next(context.FinalTargets.Count)];

        context.FinalTargets.Clear();
        context.FinalTargets.Add(target);

        var level = (int)(context.Source.Stats.TotalKnowledge / 3) >= 1 ? (int)(context.Source.Stats.TotalKnowledge / 3) : 1;
        var superForceEffect = new SuperForceEffect(level, 3);

        var effectPlan = new StatusEffectPlan(superForceEffect);

        if (context.CanOutgoingModifiers)
            ApplyStatusEffectModifiers(context, effectPlan);

        if (!ValidateTargets(context) || !ValidateStatusEffectPlan(context, effectPlan))
            return;

        target.Stats.BonusCriticalDamageMultiplier += 0.1f;
        target.ReceivePureHeal(level);
        target.ApplyStatusEffect(context, effectPlan);
    }
}
