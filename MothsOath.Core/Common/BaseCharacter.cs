using MothsOath.Core.Common.EffectInterfaces;
using MothsOath.Core.Common.EffectInterfaces.Combat;
using MothsOath.Core.Common.EffectInterfaces.Death;
using MothsOath.Core.Common.EffectInterfaces.Healing;
using MothsOath.Core.Common.EffectInterfaces.StatusEffect;
using MothsOath.Core.Common.Plans;
using MothsOath.Core.Models.Enums;
using MothsOath.Core.PassiveEffects;
using MothsOath.Core.States;
using MothsOath.Core.StatusEffect;

namespace MothsOath.Core.Common;

public abstract class BaseCharacter
{
    public Guid Id { get; protected set; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;

    public Stats Stats { get; set; } = new Stats();

    public bool IsTransformed { get; set; } = false;

    public CombatPosture Posture { get; set; } = CombatPosture.Neutral;

    public Allegiance Allegiance { get; set; }

    public List<GameTag> Tags { get; set; } = new List<GameTag>();

    public List<BasePassiveEffect> PassiveEffects { get; set; } = new List<BasePassiveEffect>();
    public List<BaseStatusEffect> StatusEffects { get; set; } = new List<BaseStatusEffect>();

    public event Action<BaseCharacter, int> OnDamageTaken;

    public virtual void Restore()
    {
        Stats.CurrentHealth = Math.Min(Stats.CurrentHealth + Stats.Regeneration, Stats.TotalMaxHealth);

        this.Stats.TemporaryMaxHealth = 0;
        this.Stats.TemporaryStrength = 0;
        this.Stats.TemporaryKnowledge = 0;
        this.Stats.TemporaryDefense = 0;
        this.Stats.TemporaryCriticalChance = 0;
        this.Stats.TemporaryCriticalDamageMultiplier = 0;
    }

    public virtual void Clean()
    {
        this.Stats.TemporaryMaxHealth = 0;
        this.Stats.TemporaryStrength = 0;
        this.Stats.TemporaryKnowledge = 0;
        this.Stats.TemporaryDefense = 0;
        this.Stats.TemporaryCriticalChance = 0;
        this.Stats.TemporaryCriticalDamageMultiplier = 0;

        this.Stats.BonusMaxHealth = 0;
        this.Stats.BonusStrength = 0;
        this.Stats.BonusKnowledge = 0;
        this.Stats.BonusDefense = 0;
        this.Stats.BonusCriticalChance = 0;
        this.Stats.BonusCriticalDamageMultiplier = 0;
        this.Stats.Shield = 0;

        this.StatusEffects.Clear();
    }

    public void HandleHealthModifier(ActionContext context, HealthModifierPlan plan)
    {
        if (context.CanIncomingModifiers)
        {
            var incomingModifiers = (this.StatusEffects ?? Enumerable.Empty<BaseStatusEffect>())
                .OfType<IIncomingHealthModifierReactor>()
                .Concat((this.PassiveEffects ?? Enumerable.Empty<BasePassiveEffect>())
                    .OfType<IIncomingHealthModifierReactor>())
                .OrderByDescending(m => m.Priority);

            foreach (var modifier in incomingModifiers)
            {
                modifier.ModifyIncomingHealthModifier(context, plan, this);
            }
        }

        if (!plan.CanProceed || plan.FinalValue == 0)
            return;

        if (plan.ModifierType == HealthModifierType.Damage)
        {
            ReceiveDamage(context, plan);
        }
        else if (plan.ModifierType == HealthModifierType.Healing)
        {
            ReceiveHeal(context, plan);
        }
    }

    public void ReceivePureDamage(int amount)
    {
        if (amount <= 0)
            return;
        Stats.CurrentHealth -= amount;
        OnDamageTaken?.Invoke(this, amount);
    }

    public void ReceiveDamage(ActionContext context, HealthModifierPlan plan)
    {
        if (!plan.BypassResistance)
            plan.FinalValue = CalculateDamageAmount(plan.FinalValue);

        if (plan.FinalValue == 0)
            return;

        int healthBefore = Stats.CurrentHealth;

        var mortuaryPlan = null as MortuaryPlan;
        if ((healthBefore - plan.FinalValue) <= 0 && context.CanDealtReactors)
        {
            mortuaryPlan = new MortuaryPlan
            {
                HealthModifierPlan = plan,
                HealthBeforeDeath = healthBefore,
                ExcessDamage = plan.FinalValue - healthBefore
            };
        }

        Stats.CurrentHealth -= plan.FinalValue;

        if (context.CanRecievedReactors)
        {
            var damageReactors = (this.StatusEffects ?? Enumerable.Empty<BaseStatusEffect>())
                .OfType<IHealthModifierReactor>()
                .Concat((this.PassiveEffects ?? Enumerable.Empty<BasePassiveEffect>())
                    .OfType<IHealthModifierReactor>())
                .OrderByDescending(m => m.Priority);

            foreach (var effect in damageReactors)
            {
                effect.ReactHealthModified(context, plan, this);
            }
        }

        if (context.CanDealtReactors)
        {
            var sourceDamageReactors = (context.Source.StatusEffects ?? Enumerable.Empty<BaseStatusEffect>())
                .OfType<IModifiedHealthReactor>()
                .Concat((context.Source.PassiveEffects ?? Enumerable.Empty<BasePassiveEffect>())
                    .OfType<IModifiedHealthReactor>())
                .OrderByDescending(m => m.Priority);

            foreach (var effect in sourceDamageReactors)
            {
                effect.OnHealthModifierApplied(context, plan, this);
            }
        }

        if (Stats.IsAlive || !context.CanDeathReactors || mortuaryPlan == null)
            return;

        CallDeathEffects(context, mortuaryPlan);
    }

    public void ReceivePureHeal(int amount)
    {
        if (amount <= 0)
            return;
        Stats.CurrentHealth = Math.Min(Stats.CurrentHealth + amount, Stats.TotalMaxHealth);
    }

    public void ReceiveHeal(ActionContext context, HealthModifierPlan plan)
    {
        int healthBefore = Stats.CurrentHealth;

        var mortuaryPlan = null as MortuaryPlan;
        if (Stats.CurrentHealth + plan.FinalValue <= 0 && context.CanDealtReactors)
        {
            mortuaryPlan = new MortuaryPlan
            {
                HealthModifierPlan = plan,
                HealthBeforeDeath = healthBefore,
                ExcessDamage = plan.FinalValue - healthBefore
            };
        }

        Stats.CurrentHealth = Math.Min(Stats.CurrentHealth + plan.FinalValue, Stats.TotalMaxHealth);

        int actualHealedAmount = Stats.CurrentHealth - healthBefore;
        plan.FinalValue = actualHealedAmount;

        if (context.CanRecievedReactors)
        {
            var healthReactors = (this.StatusEffects ?? Enumerable.Empty<BaseStatusEffect>())
                .OfType<IHealthModifierReactor>()
                .Concat((this.PassiveEffects ?? Enumerable.Empty<BasePassiveEffect>())
                    .OfType<IHealthModifierReactor>())
                .OrderByDescending(m => m.Priority);

            foreach (var effect in healthReactors)
            {
                effect.ReactHealthModified(context, plan, this);
            }
        }

        if (context.CanDealtReactors)
        {
            var sourceDamageReactors = (context.Source.StatusEffects ?? Enumerable.Empty<BaseStatusEffect>())
                .OfType<IModifiedHealthReactor>()
                .Concat((context.Source.PassiveEffects ?? Enumerable.Empty<BasePassiveEffect>())
                    .OfType<IModifiedHealthReactor>())
                .OrderByDescending(m => m.Priority);

            foreach (var effect in sourceDamageReactors)
            {
                effect.OnHealthModifierApplied(context, plan, this);
            }
        }

        if (Stats.IsAlive || !context.CanDeathReactors || mortuaryPlan == null)
            return;

        CallDeathEffects(context, mortuaryPlan);
    }

    public void ApplyPureStatusEffect(BaseStatusEffect statusEffect)
    {
        if (StatusEffects?.Any(se => se.Id == statusEffect.Id) == true)
        {
            var existingEffect = StatusEffects.First(se => se.Id == statusEffect.Id);
            existingEffect.StackEffect(this, statusEffect);
            return;
        }

        StatusEffects?.Add(statusEffect);
    }

    public void ApplyStatusEffect(ActionContext context, StatusEffectPlan plan)
    {
        if (context.CanIncomingModifiers)
        {
            var incomingModifiers = (this.StatusEffects ?? Enumerable.Empty<BaseStatusEffect>())
                .OfType<IIncomingStatusEffectModifier>()
                .Concat((this.PassiveEffects ?? Enumerable.Empty<BasePassiveEffect>())
                    .OfType<IIncomingStatusEffectModifier>())
                .OrderByDescending(m => m.Priority);

            foreach (var modifier in incomingModifiers)
            {
                modifier.ModifyIncomingStatusEffect(context, plan, this);
            }
        }

        if (!context.CanProceed || !plan.StatusEffect.IsActive())
            return;

        if (StatusEffects?.Any(se => se.Id == plan.StatusEffect.Id) == true)
        {
            var existingEffect = StatusEffects.First(se => se.Id == plan.StatusEffect.Id);
            existingEffect.StackEffect(this, plan.StatusEffect);
        }
        else
        {
            StatusEffects.Add(plan.StatusEffect);
        }

        if (context.CanRecievedReactors)
        {
            var statusEffectReactorsTarget = (this.StatusEffects ?? Enumerable.Empty<BaseStatusEffect>())
                .OfType<IStatusEffectAppliedReactor>()
                .Concat((this.PassiveEffects ?? Enumerable.Empty<BasePassiveEffect>())
                    .OfType<IStatusEffectAppliedReactor>())
                .OrderByDescending(m => m.Priority);

            foreach (var reactor in statusEffectReactorsTarget)
            {
                reactor.OnStatusEffectApplied(context, plan, this);
            }
        }

        if (!context.CanRecievedReactors)
            return;

        var statusEffectReactors = context.Source.StatusEffects.OfType<IStatusEffectDoneReactor>().ToList()
            .Concat(context.Source.PassiveEffects.OfType<IStatusEffectDoneReactor>().ToList())
            .OrderByDescending(m => m.Priority);

        foreach (var reactor in statusEffectReactors)
        {
            reactor.OnStatusEffectDone(context, plan, this);
        }
    }

    private int CalculateDamageAmount(int baseAmount)
    {
        if (baseAmount <= 0)
            return 0;

        baseAmount -= Stats.TotalDefense;

        if (Stats.Shield > 0)
        {
            int absorvedDamage = Math.Min(baseAmount, Stats.Shield);
            baseAmount -= absorvedDamage;
            Stats.Shield -= absorvedDamage;
        }

        return baseAmount;
    }

    private void CallDeathEffects(ActionContext context, MortuaryPlan plan)
    {
        var deathReactors = (this.StatusEffects ?? Enumerable.Empty<BaseStatusEffect>())
            .OfType<IDeathReactor>()
            .Concat((this.PassiveEffects ?? Enumerable.Empty<BasePassiveEffect>())
                .OfType<IDeathReactor>())
            .OrderByDescending(m => m.Priority);

        foreach (var effect in deathReactors)
        {
            effect.OnDeath(context, plan, this);
        }

        var sourceKillReactors = context.Source.StatusEffects.OfType<IKillReactor>().ToList()
            .Concat(context.Source.PassiveEffects.OfType<IKillReactor>().ToList())
            .OrderByDescending(m => m.Priority);

        foreach (var effect in sourceKillReactors)
        {
            effect.OnKill(context, plan, this);
        }
    }

    public void ActivateTurnStartEffects(CombatState combatState)
    {
        var effects = (this.StatusEffects ?? Enumerable.Empty<BaseStatusEffect>())
            .OfType<ITurnStartReactor>()
            .Concat((this.PassiveEffects ?? Enumerable.Empty<BasePassiveEffect>())
                .OfType<ITurnStartReactor>())
            .OrderByDescending(m => m.Priority);

        foreach (var effect in effects)
        {
            effect.OnTurnStart(this, combatState);
        }
    }

    public void ActivateTurnEndEffects(CombatState combatState)
    {
        var effects = (this.StatusEffects ?? Enumerable.Empty<BaseStatusEffect>())
            .OfType<ITurnEndReactor>()
            .Concat((this.PassiveEffects ?? Enumerable.Empty<BasePassiveEffect>())
                .OfType<ITurnEndReactor>())
            .OrderByDescending(m => m.Priority);

        foreach (var effect in effects)
        {
            effect.OnTurnEnd(this, combatState);
        }
    }

    public void ClearFadingStatusEffects(CombatState context)
    {
        if (!StatusEffects.Any())
            return;

        var fadedEffects = this.StatusEffects.Where(se => se.Duration <= 0).ToList();

        var effects = fadedEffects.OfType<IFadingReactor>().ToList();
        foreach (var effect in effects)
        {
            effect.OnFading(this, context);
        }

        StatusEffects.RemoveAll(se => !se.IsActive());
    }
}