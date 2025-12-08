using MothsOath.Core.Common;
using MothsOath.Core.Common.EffectInterfaces.Turn;
using MothsOath.Core.Models.Enums;
using MothsOath.Core.States;

namespace MothsOath.Core.StatusEffect.ConcreteStatusEffects;

public class StrongBodyEffect : BaseStatusEffect, ITurnStartReactor
{
    public override string Id { get; set; } = "strong_body_effect";

    public override string Name { get; set; } = "Corpo Forte";

    public override string Description { get; set; } = "Aumenta a força temporariamente.";

    public override bool IsEndless { get; set; } = false;

    public override bool IsEchoable { get; set; } = true;

    public override bool IsVisible { get; set; } = true;

    public override StatusEffectType EffectType { get; set; } = StatusEffectType.Positive;

    public StrongBodyEffect(int level, int duration)
    {
        Level = level;
        Duration = duration;
    }

    public void OnTurnStart(Character target, CombatState context)
    {
       var effectLevel = (int)(this.Level / 2) > 1 ? (int)(this.Level / 2) : 2;
       target.Stats.BonusStrength += effectLevel;
    }
}
