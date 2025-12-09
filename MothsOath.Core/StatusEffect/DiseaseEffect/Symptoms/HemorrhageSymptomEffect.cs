using MothsOath.Core.Common;
using MothsOath.Core.Common.EffectInterfaces.Turn;
using MothsOath.Core.States;
using MothsOath.Core.StatusEffect.ConcreteEffects;

namespace MothsOath.Core.StatusEffect.DiseaseEffect.Symptoms;

public class HemorrhageSymptomEffect : BaseSymptomEffect, ITurnStartReactor
{
    public override string Id { get; set; } = "hemorrhage_symptom";

    public override string Name { get; set; } = "Hemorragia";

    public override string Description { get; set; } = "Causa sangramento.";

    public int Priority { get; set; } = 0;

    public HemorrhageSymptomEffect()
    {
    }

    public void OnTurnStart(BaseCharacter target, CombatState context)
    {
        var disease = GetTargetDisease(target);
        var level = disease != null ? disease.Level : 1;

        var rng = new Random();
        var chance = 1 + level;

        if (rng.Next(1, 101) <= chance)
        {
            target.ApplyPureStatusEffect(new BleedingEffect(level, 1));
        }

    }
}
