using MothsOath.Core.Common;
using MothsOath.Core.Entities.Archetypes;
using MothsOath.Core.States;

namespace MothsOath.Core.StatusEffect.DiseaseEffect.Symptoms;

public abstract class BaseSymptomEffect
{
    public abstract string Id { get; set; }

    public abstract string Name { get; set; }

    public abstract string Description { get; set; }

    public BaseSymptomEffect()
    {
    }

    public DiseaseEffect? GetDoctorDisease(CombatState combatState)
    {
        if(combatState.Player is Doctor doctor)
        {
            return doctor.Disease;
        }

        return null;
    }

    public DiseaseEffect? GetTargetDisease(Character target)
    {
        return target.StatusEffects.Find(se => se is DiseaseEffect) as DiseaseEffect;
    }
}
