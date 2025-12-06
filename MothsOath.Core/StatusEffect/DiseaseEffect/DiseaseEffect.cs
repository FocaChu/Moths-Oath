using MothsOath.Core.Behaviors;
using MothsOath.Core.Common;
using MothsOath.Core.Common.EffectInterfaces;
using MothsOath.Core.States;
using MothsOath.Core.StatusEffect.DiseaseEffect.Symptoms;

namespace MothsOath.Core.StatusEffect.DiseaseEffect;

public class DiseaseEffect : BaseStatusEffect, ITurnEndReactor
{
    public override string Id { get; set; }

    public override string Name { get; set; }

    public override string Description { get; set; }

    public override bool IsEndless { get; set; } = false;

    public IBehavior Behavior { get; set; } 

    public List<BaseSymptomEffect> Symptoms { get; set; } = new List<BaseSymptomEffect>();

    public List<BaseSymptomEffect> AvaliebleMutations { get; set; } = new List<BaseSymptomEffect>();

    public DiseaseEffect(string id, string name, string description, int level, int duration, bool isEndless, IBehavior behavior, List<BaseSymptomEffect> symptoms)
    {
        Id = id;
        Name = name;
        Description = description;
        Level = level;
        Duration = duration;
        IsEndless = isEndless;
        Behavior = behavior;
        Symptoms = symptoms;
        Duration = duration;
        IsEndless = isEndless;
        Symptoms = symptoms;
    }

    public DiseaseEffect(string id, string name, string description, bool isEndless, IBehavior behavior, List<BaseSymptomEffect> symptoms, List<BaseSymptomEffect> avaliebleMutations)
    {
        Id = id;
        Level = 3;
        Duration = 6;
        IsEndless = false;
        Name = name;
        Description = description;
        IsEndless = isEndless;
        Behavior = behavior;
        Symptoms = symptoms;
        AvaliebleMutations = avaliebleMutations;
    }

    public override void StackEffect(Character owner, BaseStatusEffect newEffect)
    {
        owner.StatusEffects.Remove(this);
        owner.StatusEffects.Add(newEffect);
    }

    public override void TickTime(Character holder)
    {
        if (Duration > 0 && !IsEndless)
        {
            var rng = new Random();

            var chance = 10 + Level * 5;

            if (rng.Next(1, 101) >= chance)
                Duration--;
        }
    }

    public DiseaseEffect Clone()
    {
        return new DiseaseEffect(this.Id, this.Name, this.Description, this.Level, this.Duration, this.IsEndless, this.Behavior, this.Symptoms.ToList());
    }

    public void OnTurnEnd(Character target, CombatState context)
    {
        var effects = Symptoms.OfType<ITurnEndReactor>().ToList();
        foreach (var effect in effects)
        {
            effect.OnTurnEnd(target, context);
        }
    }
}
