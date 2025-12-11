using MothsOath.Core.Abilities;
using MothsOath.Core.Common;
using MothsOath.Core.Common.Plans;
using MothsOath.Core.Entities;
using MothsOath.Core.StatusEffect.ConcreteEffects;
namespace MothsOath.Core.Actions;

public class SilenceAction : BaseAction
{
    public override string Id => "action_silence";

    public override void Execute(ActionContext context)
    {
        context.FinalTargets.RemoveAll(t => t is Player);

        if (!ValidateTargets(context))
            return;

        var rng = new Random();
        var target = context.FinalTargets[rng.Next(context.FinalTargets.Count)];

        context.FinalTargets.Clear();
        context.FinalTargets.Add(target);

        var duration = (int)(context.Source.Stats.TotalKnowledge / 10) >= 2 ? (int)(context.Source.Stats.TotalKnowledge / 10) : 2;
        var silenceEffect = new SilenceEffect(duration);

        var effectPlan = new StatusEffectPlan(silenceEffect);

        if (context.CanOutgoingModifiers)
            ApplyStatusEffectModifiers(context, effectPlan);

        if (!ValidateTargets(context) || !ValidateStatusEffectPlan(context, effectPlan))
            return;

        target.ApplyStatusEffect(context, effectPlan);

        Console.WriteLine($"{context.Source.Name} silencia {target.Name}!");
    }
}
