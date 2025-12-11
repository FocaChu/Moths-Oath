using MothsOath.Core.Common;
using MothsOath.Core.Common.EffectInterfaces.Healing;
using MothsOath.Core.Common.Plans;
using MothsOath.Core.Models.Enums;

namespace MothsOath.Core.StatusEffect.ConcreteEffects;

public class TauntedEffect : BaseStatusEffect, IOutgoingHealthModifierReactor
{
    public override string Id { get; set; } = "taunted_effect";

    public override string Name { get; set; } = "Provocado";

    public override string Description { get; set; } = "Força o alvo a atacar seu provocador.";

    public override bool IsEndless { get; set; } = false;

    public override bool IsEchoable { get; set; } = false;

    public override bool IsVisible { get; set; } = true;

    public int Priority { get; set; } = 1;

    public override StatusEffectType EffectType { get; set; } = StatusEffectType.Negative;

    public BaseCharacter Source { get; set; }

    public TauntedEffect(int level, int duration, BaseCharacter source)
    {
        Level = level;
        Duration = duration;
        Source = source;
    }

    public void ModifyOutgoingHealthModifier(ActionContext context, HealthModifierPlan plan)
    {
        var targets = context.FinalTargets;
        if (targets.Count == 0 || !targets.Contains(Source))
            return;

        var rng = Random.Shared;
        var chance = Level * 10;
        var roll = rng.Next(1, 101);

        if (roll > chance)
            return;

        context.FinalTargets = new List<BaseCharacter> { Source };
    }
}
