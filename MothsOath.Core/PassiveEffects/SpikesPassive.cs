using MothsOath.Core.Common;
using MothsOath.Core.Common.EffectInterfaces.Damage;
using MothsOath.Core.Common.Plans;

namespace MothsOath.Core.PassiveEffects;

public class SpikesPassive : BasePassiveEffect, IDamageReceivedReactor
{
    public override string Id { get; set; } = "spikes_passive";

    public override string Name { get; set; } = "Espinhos";

    public override string Description { get; set; } = "Causa dano a agressores.";

    public void OnDamageReceived(ActionContext context, DamagePlan plan, Character target)
    {
        if (plan.CanProceed && plan.FinalDamageAmount > 0)
        {
            int spikeDamage = (int)(target.Stats.TotalResistance / 2) + (int)(target.Stats.TotalStrength / 4);

            var damagePlan = new DamagePlan(spikeDamage, true);

            context.CanDealtReactors = false;
            context.CanRecievedReactors = false;

            context.Source.RecieveDamage(context, damagePlan);
        }
    }

}
