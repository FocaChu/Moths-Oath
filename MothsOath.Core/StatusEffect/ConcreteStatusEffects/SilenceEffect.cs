using MothsOath.Core.Common;
using MothsOath.Core.Common.EffectInterfaces.Combat;
using MothsOath.Core.Common.EffectInterfaces.Healing;
using MothsOath.Core.Common.Plans;
using MothsOath.Core.Models.Enums;
using MothsOath.Core.States;

namespace MothsOath.Core.StatusEffect.ConcreteEffects;

public class SilenceEffect : BaseStatusEffect, IOutgoingHealthModifierReactor, ITurnEndReactor
{
    public override string Id { get; set; } = "silence_effect";

    public override string Name { get; set; } = "Silêncio";

    public override string Description { get; set; } = "Impede o alvo de usar sua habilidade especial.";

    public override bool IsEndless { get; set; } = false;

    public override bool IsEchoable { get; set; } = false;

    public override bool IsVisible { get; set; } = true;

    public int Priority { get; set; } = 1;

    public override StatusEffectType EffectType { get; set; } = StatusEffectType.Negative;

    public SilenceEffect(int duration)
    {
        Level = 1;
        Duration = duration;
    }

    public void ModifyOutgoingHealthModifier(ActionContext context, HealthModifierPlan plan)
    {
        context.CanUseSpecial = false;
    }

    public void OnTurnEnd(BaseCharacter target, CombatState context)
    {
        base.TickTime(target);
    }
}
