using MothsOath.Core.Common;
using MothsOath.Core.Common.EffectInterfaces;
using MothsOath.Core.States;

namespace MothsOath.Core.StatusEffect.DiseaseEffect.Symptoms;

public class NecrosisSymptomEffect : BaseSymptomEffect, ITurnEndReactor
{
    public override string Id { get; set; } = "necrosis_symptom";

    public override string Name { get; set; } = "Necrose";

    public override string Description { get; set; } = "Inflinge dano ao longo do tempo.";

    public NecrosisSymptomEffect(string id, string name, string description) : base(id, name, description)
    {
    }

    public void OnTurnEnd(Character target, CombatState context)
    {
        var disease = GetTargetDisease(target);

        var bonusDamage = disease != null ? disease.Level : 1;

        var damageAmount = (int)(target.Stats.MaxHealth / 20) + bonusDamage;

        target.RecievePureDamage(damageAmount);
    }
}
