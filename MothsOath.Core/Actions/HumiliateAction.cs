using MothsOath.Core.Abilities;
using MothsOath.Core.Common;
using MothsOath.Core.Common.Plans;
using MothsOath.Core.Entities;
using MothsOath.Core.StatusEffect.ConcreteEffects;
namespace MothsOath.Core.Actions;

public class HumiliateAction : BaseAction
{
    public override string Id => "action_humiliate";

    public override void Execute(ActionContext context)
    {
        if (!ValidateTargets(context))
            return;

        var rng = new Random();
        var target = context.FinalTargets[rng.Next(context.FinalTargets.Count)];

        context.FinalTargets.Clear();
        context.FinalTargets.Add(target);

        target.Stats.TemporaryCriticalChance--;
        target.Stats.TemporaryKnowledge--;

        Console.WriteLine($"{context.Source.Name} humilha {target.Name}!");

        if (target is Player)
            return;

        var level = (int)(context.Source.Stats.TotalKnowledge / 2) >= 2 ? (int)(context.Source.Stats.TotalKnowledge / 2) : 1;
        var tauntedEffect = new TauntedEffect(level, 3, context.Source);

        var effectPlan = new StatusEffectPlan(tauntedEffect);

        if (context.CanOutgoingModifiers)
            ApplyStatusEffectModifiers(context, effectPlan);

        if (!ValidateTargets(context) || !ValidateStatusEffectPlan(context, effectPlan))
            return;

        target.ApplyStatusEffect(context, effectPlan);

    }
}
