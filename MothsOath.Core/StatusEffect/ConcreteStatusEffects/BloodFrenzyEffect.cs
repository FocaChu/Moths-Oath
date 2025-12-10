using MothsOath.Core.Common;
using MothsOath.Core.Common.EffectInterfaces.Death;
using MothsOath.Core.Models.Enums;

namespace MothsOath.Core.StatusEffect.ConcreteEffects;

public class BloodFrenzyEffect : BaseStatusEffect, IKillReactor
{
    public override string Id { get; set; } = "blood_frenzy_effect";

    public override string Name { get; set; } = "Frenesi Sanguinário";

    public override string Description { get; set; } = "Restaura vida ao matar uma entidade.";

    public override bool IsEndless { get; set; } = false;

    public override bool IsEchoable { get; set; } = true;

    public override bool IsVisible { get; set; } = true;

    public int Priority { get; set; } = 1;

    public override StatusEffectType EffectType { get; set; } = StatusEffectType.Positive;

    public BloodFrenzyEffect(int level, int duration)
    {
        Level = level;
        Duration = duration;
    }

    public void OnKill(ActionContext context, BaseCharacter victim)
    {
        var killer = context.Source;

        killer.Stats.CurrentHealth += Level;
        Console.WriteLine($"{killer.Name} restaura {Level} de vida ao matar {victim.Name}. HP:{killer.Stats.CurrentHealth}");

    }
}
