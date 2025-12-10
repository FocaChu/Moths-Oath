using MothsOath.Core.Common;
using MothsOath.Core.Common.EffectInterfaces.Damage;
using MothsOath.Core.Common.Plans;

namespace MothsOath.Core.PassiveEffects;

public class SpikesPassive : BasePassiveEffect, IDamageReceivedReactor
{
    public override string Id { get; set; } = "spikes_passive";

    public override string Name { get; set; } = "Espinhos";

    public override string Description { get; set; } = "Causa dano a agressores.";

    public int Priority { get; set; } = 0;

    public void OnDamageReceived(ActionContext context, HealthModifierPlan plan, BaseCharacter target)
    {
        if (plan.CanProceed && plan.FinalValue > 0)
        {
            int spikeDamage = (int)(target.Stats.TotalDefense / 2) + (int)(target.Stats.TotalStrength / 4);

            var damagePlan = new HealthModifierPlan(spikeDamage);

            context.CanDealtReactors = false;
            context.CanRecievedReactors = false;

            context.Source.ReceiveDamage(context, damagePlan);
        }
    }

}
