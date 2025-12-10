using MothsOath.Core.Behaviors;
using MothsOath.Core.Common;
using MothsOath.Core.Common.EffectInterfaces;
using MothsOath.Core.Common.EffectInterfaces.Damage;
using MothsOath.Core.Common.EffectInterfaces.Healing;
using MothsOath.Core.Common.EffectInterfaces.StatusEffect;
using MothsOath.Core.Common.EffectInterfaces.Combat;
using MothsOath.Core.Common.Plans;
using MothsOath.Core.Models.Enums;
using MothsOath.Core.States;
using MothsOath.Core.StatusEffect.DiseaseEffect.Symptoms;
using System.Numerics;

namespace MothsOath.Core.StatusEffect.DiseaseEffect;

public class DiseaseEffect : BaseStatusEffect, ICombatStartReactor, ITurnEndReactor, ITurnStartReactor, IFadingReactor,
                                               IGlobalDamageInteractor, IGlobalHealingInteractor, IGlobalStatusEffectInteractor,
                                               IActionPlanModifier
{
    public override string Id { get; set; }

    public override string Name { get; set; }

    public override string Description { get; set; }

    public override bool IsEndless { get; set; } = false;

    public override bool IsEchoable { get; set; } = false;

    public override bool IsVisible { get; set; } = true;

    public override StatusEffectType EffectType { get; set; } = StatusEffectType.Disease;

    public IBehavior Behavior { get; set; }

    public int Priority { get; set; } = 1;


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

    public override void StackEffect(BaseCharacter owner, BaseStatusEffect newEffect)
    {
        owner.StatusEffects.Remove(this);
        owner.StatusEffects.Add(newEffect);
    }

    public override void TickTime(BaseCharacter holder)
    {
        if (Duration > 0 && !IsEndless)
        {
            var rng = new Random();

            var chance = 10 + Level * 5;

            if (rng.Next(1, 101) >= chance)
                Duration--;
        }
    }

    public override DiseaseEffect Clone()
    {
        return new DiseaseEffect(this.Id, this.Name, this.Description, this.Level, this.Duration, this.IsEndless, this.Behavior, this.Symptoms.ToList());
    }

    public override BaseStatusEffect EchoEffect()
    {
        return null;
    }

    public void OnTurnEnd(BaseCharacter target, CombatState context)
    {
        var effects = Symptoms.OfType<ITurnEndReactor>().ToList().OrderByDescending(e => e.Priority);
        foreach (var effect in effects)
        {
            effect.OnTurnEnd(target, context);
        }
    }

    public void OnTurnStart(BaseCharacter target, CombatState context)
    {
        var effects = Symptoms.OfType<ITurnStartReactor>().ToList().OrderByDescending(e => e.Priority);
        foreach (var effect in effects)
        {
            effect.OnTurnStart(target, context);
        }
    }

    public void ModifyActionPlan(ActionContext context)
    {
        var effects = Symptoms.OfType<IActionPlanModifier>().ToList().OrderByDescending(e => e.Priority);
        foreach (var effect in effects)
        {
            effect.ModifyActionPlan(context);
        }
    }

    public void OnDamageDealt(ActionContext context, HealthModifierPlan plan, BaseCharacter target)
    {
        var effects = Symptoms.OfType<IDamageDealtReactor>().ToList().OrderByDescending(e => e.Priority);
        foreach (var effect in effects)
        {
            effect.OnDamageDealt(context, plan, target);
        }
    }

    public void OnDamageReceived(ActionContext context, HealthModifierPlan plan, BaseCharacter target)
    {
        var effects = Symptoms.OfType<IDamageReceivedReactor>().ToList().OrderByDescending(e => e.Priority);
        foreach (var effect in effects)
        {
            effect.OnDamageReceived(context, plan, target);
        }
    }

    public void ModifyIncomingDamage(ActionContext context, HealthModifierPlan plan, BaseCharacter target)
    {
        var effects = Symptoms.OfType<IIncomingDamageModifier>().ToList().OrderByDescending(e => e.Priority);
        foreach (var effect in effects)
        {
            effect.ModifyIncomingDamage(context, plan, target);
        }
    }

    public void ModifyOutgoingDamage(ActionContext context, HealthModifierPlan plan)
    {
        var effects = Symptoms.OfType<IOutgoingDamageModifier>().ToList().OrderByDescending(e => e.Priority);
        foreach (var effect in effects)
        {
            effect.ModifyOutgoingDamage(context, plan);
        }
    }

    public void OnHealingDone(ActionContext context, HealthModifierPlan plan, BaseCharacter target)
    {
        var effects = Symptoms.OfType<IHealingDoneReactor>().ToList().OrderByDescending(e => e.Priority);
        foreach (var effect in effects)
        {
            effect.OnHealingDone(context, plan, target);
        }
    }

    public void OnHealingReceived(ActionContext context, HealthModifierPlan plan, BaseCharacter target)
    {
        var effects = Symptoms.OfType<IHealingReceivedReactor>().ToList().OrderByDescending(e => e.Priority);
        foreach (var effect in effects)
        {
            effect.OnHealingReceived(context, plan, target);
        }
    }

    public void ModifyIncomingHealing(ActionContext context, HealthModifierPlan plan, BaseCharacter target)
    {
        var effects = Symptoms.OfType<IIncomingHealingModifier>().ToList().OrderByDescending(e => e.Priority);
        foreach (var effect in effects)
        {
            effect.ModifyIncomingHealing(context, plan, target);
        }
    }

    public void ModifyOutgoingHealing(ActionContext context, HealthModifierPlan plan)
    {
        var effects = Symptoms.OfType<IOutgoingHealingModifier>().ToList().OrderByDescending(e => e.Priority);
        foreach (var effect in effects)
        {
            effect.ModifyOutgoingHealing(context, plan);
        }
    }

    public void OnStatusEffectDone(ActionContext context, StatusEffectPlan plan, BaseCharacter target)
    {
        var effects = Symptoms.OfType<IStatusEffectDoneReactor>().ToList().OrderByDescending(e => e.Priority);
        foreach (var effect in effects)
        {
            effect.OnStatusEffectDone(context, plan, target);
        }
    }

    public void OnStatusEffectApplied(ActionContext context, StatusEffectPlan plan, BaseCharacter target)
    {
        var effects = Symptoms.OfType<IStatusEffectAppliedReactor>().ToList().OrderByDescending(e => e.Priority);
        foreach (var effect in effects)
        {
            effect.OnStatusEffectApplied(context, plan, target);
        }
    }

    public void ModifyIncomingStatusEffect(ActionContext context, StatusEffectPlan plan, BaseCharacter target)
    {
        var effects = Symptoms.OfType<IIncomingStatusEffectModifier>().ToList().OrderByDescending(e => e.Priority);
        foreach (var effect in effects)
        {
            effect.ModifyIncomingStatusEffect(context, plan, target);
        }
    }

    public void ModifyOutgoingStatusEffect(ActionContext context, StatusEffectPlan plan)
    {
        var effects = Symptoms.OfType<IOutgoingStatusEffectModifier>().ToList().OrderByDescending(e => e.Priority);
        foreach (var effect in effects)
        {
            effect.ModifyOutgoingStatusEffect(context, plan);
        }
    }

    public void OnCombatStart(BaseCharacter target, CombatState context)
    {
        var effects = Symptoms.OfType<ICombatStartReactor>().ToList().OrderByDescending(e => e.Priority);
        foreach (var effect in effects)
        {
            effect.OnCombatStart(target, context);
        }
    }

    public void OnFading(BaseCharacter target, CombatState context)
    {
        var effects = Symptoms.OfType<IFadingReactor>().ToList().OrderByDescending(e => e.Priority);
        foreach (var effect in effects)
        {
            effect.OnFading(target, context);
        }
    }
}
