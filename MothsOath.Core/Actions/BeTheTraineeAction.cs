using MothsOath.Core.Common;
using MothsOath.Core.Common.Plans;
using MothsOath.Core.Models.Enums;

namespace MothsOath.Core.Abilities;

public class BeTheTraineeAction : BaseAction
{
    public override string Id => "action_be_the_trainee";

    public override void Execute(ActionContext context)
    {
        AssistAction(context);
    }

    private void AssistAction(ActionContext context)
    {
        var source = context.Source;
        var value = (int)(source.Stats.TotalKnowledge / 2) >= 3 ? (int)(source.Stats.TotalKnowledge / 2) : 3;

        var allies = context.BaseTargets.Where(t => t.Allegiance == context.Source.Allegiance && t.Stats.IsAlive).ToList();

        if(allies.Count == 0)
            return;

        var rng = Random.Shared;
        var target = allies[rng.Next(allies.Count)];

        var healValue = value + target.Stats.Regeneration;
        var healPlan = new HealthModifierPlan(healValue, HealthModifierType.Healing);

        target.ReceiveHeal(context, healPlan);
        target.Stats.Shield += healValue;

        target.Stats.BonusCriticalChance++;
        target.Stats.BaseCriticalDamageMultiplier += 0.02f;
    }
}
