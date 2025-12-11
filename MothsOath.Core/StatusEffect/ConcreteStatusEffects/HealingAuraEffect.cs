using MothsOath.Core.Common;
using MothsOath.Core.Common.EffectInterfaces.Combat;
using MothsOath.Core.Models.Enums;
using MothsOath.Core.States;

namespace MothsOath.Core.StatusEffect.ConcreteStatusEffects;

public class HealingAuraEffect : BaseStatusEffect, ITurnStartReactor
{
    public override string Id { get; set; } = "healing_aura_effect";

    public override string Name { get; set; } = "Aura de Cura";

    public override string Description { get; set; } = "Cura o portador no início de cada turno.";

    public override bool IsEndless { get; set; } = false;

    public override bool IsEchoable { get; set; } = true;

    public override bool IsVisible { get; set; } = true;

    public int Priority { get; set; } = 1;

    public override StatusEffectType EffectType { get; set; } = StatusEffectType.Positive;

    public HealingAuraEffect(int level, int duration)
    {
        Level = level;
        Duration = duration;
    }

    public void OnTurnStart(BaseCharacter target, CombatState context)
    {
        target.ReceivePureHeal(this.Level);
    }
}
