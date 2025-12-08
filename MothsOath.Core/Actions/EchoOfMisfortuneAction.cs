using MothsOath.Core.Abilities;
using MothsOath.Core.Common;
using MothsOath.Core.Common.Plans;
using MothsOath.Core.StatusEffect;

namespace MothsOath.Core.Actions;

public class EchoOfMisfortuneAction : BaseAction
{
    public override string Id => "action_echo_of_misfortune";

    public override void Execute(ActionContext context)
    {
        if (!ValidadeTargets(context))
            return;

        var rng = new Random();
        var target = context.FinalTargets[rng.Next(context.FinalTargets.Count)];

        context.FinalTargets.Clear();
        context.FinalTargets.Add(target);

        var availableStatusEffects = target.StatusEffects.Where(se => se.IsEchoable).ToList();


        var statusEffect = availableStatusEffects.Count > 0
            ? availableStatusEffects[rng.Next(availableStatusEffects.Count)]
            : null;

        if (statusEffect == null)
            return;

        var echoEffect = new EchoStatusEffect(statusEffect.Clone());
        var effectPlan = new StatusEffectPlan(echoEffect);

        if (context.CanOutgoingModifiers)
            ApplyStatusEffectModifiers(context, effectPlan);

        if (!ValidadeTargets(context) || !ValidateStatusEffectPlan(context, effectPlan))
            return;

        target.ApplyStatusEffect(context, effectPlan);

        Console.WriteLine($"{context.Source.Name} echoes the misfortune of {target.Name}! {echoEffect.Name}");
    }

    public override bool ValidateStatusEffectPlan(ActionContext context, StatusEffectPlan plan)
    {
        var echo = plan.StatusEffect as EchoStatusEffect;

        if (echo == null || echo._echoedEffect == null || !echo._echoedEffect.IsActive() || !context.CanProceed)
            return false;

        return true;
    }
}
