using MothsOath.Core.Common;
using MothsOath.Core.Common.EffectInterfaces.Death;
using MothsOath.Core.Common.Plans;

namespace MothsOath.Core.PassiveEffects;

public class VampirismPassiveEffect : BasePassiveEffect, IKillReactor
{
    public override string Id { get; set; } = "vampirism_passive";

    public override string Name { get; set; } = "Vampirismo";

    public override string Description { get; set; } = "Se regenera ao matar inimigos.";

    public int Priority { get; set; } = 2;

    public void OnKill(ActionContext context, MortuaryPlan plan, BaseCharacter victim)
    {
        var source = context.Source;
        var heal = source.Stats.Regeneration + victim.Stats.Regeneration;

        source.ReceivePureHeal(heal);
    }
}
