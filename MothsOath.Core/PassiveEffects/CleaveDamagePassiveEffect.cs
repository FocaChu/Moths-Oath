using MothsOath.Core.Common;
using MothsOath.Core.Common.EffectInterfaces.Death;
using MothsOath.Core.Common.Plans;

namespace MothsOath.Core.PassiveEffects;

public class CleaveDamagePassiveEffect : BasePassiveEffect, IKillReactor
{
    public override string Id { get; set; } = "cleave_damage_passive";

    public override string Name { get; set; } = "Dano em Varrimento";

    public override string Description { get; set; } = "Dano excedente em ataques é distribuído entre inimigos próximos.";

    public int Priority { get; set; } = 1;

    public void OnKill(ActionContext context, MortuaryPlan plan, BaseCharacter target)
    {
        // Implementar a lógica para distribuir o dano excedente entre inimigos próximos
    }
}
