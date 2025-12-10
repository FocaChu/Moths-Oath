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

    public Allegiance Allegiance { get; set; }

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
        this.Stats.BonusMaxHealth = 0;
        this.Stats.BonusStrength = 0;
        this.Stats.BonusKnowledge = 0;
        this.Stats.BonusDefense = 0;
        this.Stats.BonusCriticalChance = 0;
        this.Stats.BonusCriticalDamage = 0;
        this.Stats.Shield = 0;

        this.StatusEffects.Clear();
    }

    public void HandleHealthModifier(ActionContext context, HealthModifierPlan plan)
    {
        if (context.CanIncomingModifiers)
        {
            var incomingModifiers = this.StatusEffects.OfType<IIncomingHealthModifierReactor>().ToList()
                .Concat(this.PassiveEffects.OfType<IIncomingHealthModifierReactor>().ToList())
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
            RecieveHeal(context, plan);
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
            var damageReactors = this.StatusEffects.OfType<IHealthModifierReactor>().ToList()
                .Concat(this.PassiveEffects.OfType<IHealthModifierReactor>().ToList())
                .OrderByDescending(m => m.Priority);

            foreach (var effect in damageReactors)
            {
                effect.ReactHealthModified(context, plan, this);
            }
        }

        if (context.CanDealtReactors)
        {
            var sourceDamageReactors = context.Source.StatusEffects.OfType<IModifiedHealthReactor>().ToList()
                .Concat(context.Source.PassiveEffects.OfType<IModifiedHealthReactor>().ToList())
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

    public void RecievePureHeal(int amount)
    {
        if (amount <= 0)
            return;
        Stats.CurrentHealth = Math.Min(Stats.CurrentHealth + amount, Stats.TotalMaxHealth);
    }

    public void RecieveHeal(ActionContext context, HealthModifierPlan plan)
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
            var healthReactors = this.StatusEffects.OfType<IHealthModifierReactor>().ToList()
                .Concat(this.PassiveEffects.OfType<IHealthModifierReactor>().ToList())
                .OrderByDescending(m => m.Priority);

            foreach (var effect in healthReactors)
            {
                effect.ReactHealthModified(context, plan, this);
            }
        }

        if (context.CanDealtReactors)
        {
            var sourceDamageReactors = context.Source.StatusEffects.OfType<IModifiedHealthReactor>().ToList()
                .Concat(context.Source.PassiveEffects.OfType<IModifiedHealthReactor>().ToList())
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
        if (StatusEffects.Any(se => se.Id == statusEffect.Id))
        {
            var existingEffect = StatusEffects.First(se => se.Id == statusEffect.Id);
            existingEffect.StackEffect(this, statusEffect);
            return;
        }

        StatusEffects.Add(statusEffect);
    }

    public void ApplyStatusEffect(ActionContext context, StatusEffectPlan plan)
    {
        if (context.CanIncomingModifiers)
        {
            var incomingModifiers = this.StatusEffects.OfType<IIncomingStatusEffectModifier>().ToList()
                .Concat(this.PassiveEffects.OfType<IIncomingStatusEffectModifier>().ToList())
                .OrderByDescending(m => m.Priority);

            foreach (var modifier in incomingModifiers)
            {
                modifier.ModifyIncomingStatusEffect(context, plan, this);
            }
        }

        if (!context.CanProceed || !plan.StatusEffect.IsActive())
            return;

        if (StatusEffects.Any(se => se.Id == plan.StatusEffect.Id))
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
            var statusEffectReactorsTarget = this.StatusEffects.OfType<IStatusEffectAppliedReactor>().ToList()
                .Concat(this.PassiveEffects.OfType<IStatusEffectAppliedReactor>().ToList())
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
        var deathReactors = this.StatusEffects.OfType<IDeathReactor>().ToList()
            .Concat(this.PassiveEffects.OfType<IDeathReactor>().ToList())
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
        var effects = this.StatusEffects.OfType<ITurnStartReactor>().ToList()
            .Concat(this.PassiveEffects.OfType<ITurnStartReactor>().ToList())
            .OrderByDescending(m => m.Priority);

        foreach (var effect in effects)
        {
            effect.OnTurnStart(this, combatState);
        }
    }

    public void ActivateTurnEndEffects(CombatState combatState)
    {
        var effects = this.StatusEffects.OfType<ITurnEndReactor>().ToList()
            .Concat(this.PassiveEffects.OfType<ITurnEndReactor>().ToList())
            .OrderByDescending(m => m.Priority);

        foreach (var effect in effects)
        {
            effect.OnTurnEnd(this, combatState);
        }
    }

    public void TickStatusEffects(CombatState context)
    {
        if (!StatusEffects.Any())
            return;

        foreach (var statusEffect in StatusEffects)
        {
            statusEffect.TickTime(this);
        }

        ClearFadingStatusEffects(context);
    }

    public void ClearFadingStatusEffects(CombatState context)
    {

        var effects = this.StatusEffects.OfType<IFadingReactor>().ToList();
        foreach (var effect in effects)
        {
            effect.OnFading(this, context);
        }

        StatusEffects.RemoveAll(se => se.Duration <= 0);
    }
}