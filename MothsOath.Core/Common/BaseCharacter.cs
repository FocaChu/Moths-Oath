using MothsOath.Core.Common.EffectInterfaces;
using MothsOath.Core.Common.EffectInterfaces.Combat;
using MothsOath.Core.Common.EffectInterfaces.Death;
using MothsOath.Core.Common.EffectInterfaces.Health;
using MothsOath.Core.Common.EffectInterfaces.StatusEffect;
using MothsOath.Core.Common.Effects;
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

    private EffectCache? _effectCache;

    public event Action<BaseCharacter, int> OnDamageTaken;

    /// <summary>
    /// Gets the effect cache for this character. Lazy initializes if needed.
    /// </summary>
    public EffectCache GetEffectCache()
    {
        if (_effectCache == null)
        {
            _effectCache = new EffectCache(this);
        }
        return _effectCache;
    }

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
        EffectPipelineExecutor.ExecuteIncomingHealthModifiers(this, context, plan);

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

        EffectPipelineExecutor.ExecuteReceivedHealthReactors(this, context, plan);

        EffectPipelineExecutor.ExecuteDealtHealthReactors(context.Source, context, plan, this);

        if (!Stats.IsAlive && context.CanDeathReactors && mortuaryPlan != null)
        {
            EffectPipelineExecutor.ExecuteDeathEffects(this, context, mortuaryPlan);
        }
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

        EffectPipelineExecutor.ExecuteReceivedHealthReactors(this, context, plan);

        EffectPipelineExecutor.ExecuteDealtHealthReactors(context.Source, context, plan, this);

        if (!Stats.IsAlive && context.CanDeathReactors && mortuaryPlan != null)
        {
            EffectPipelineExecutor.ExecuteDeathEffects(this, context, mortuaryPlan);
        }
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
        EffectPipelineExecutor.ExecuteIncomingStatusEffectModifiers(this, context, plan);

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
            GetEffectCache().Invalidate();
        }

        EffectPipelineExecutor.ExecuteStatusEffectAppliedReactors(this, context, plan);

        EffectPipelineExecutor.ExecuteStatusEffectDoneReactors(context.Source, context, plan, this);
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
        EffectPipelineExecutor.ExecuteTurnStartEffects(this, combatState);
    }

    public void ActivateTurnEndEffects(CombatState combatState)
    {
        EffectPipelineExecutor.ExecuteTurnEndEffects(this, combatState);
    }

    public void ClearFadingStatusEffects(CombatState context)
    {
        if (!StatusEffects.Any())
            return;

        var fadedEffects = StatusEffects.Where(se => se.Duration <= 0).ToList();

        if (fadedEffects.Count > 0)
        {
            EffectPipelineExecutor.ExecuteFadingEffects(this, context, fadedEffects);
            
            StatusEffects.RemoveAll(se => !se.IsActive());
            GetEffectCache().Invalidate();
        }
    }

    /// <summary>
    /// Ticks time on all active status effects.
    /// This should be called AFTER all turn effects have been executed,
    /// but BEFORE clearing faded effects.
    /// </summary>
    public void TickAllStatusEffects()
    {
        foreach (var statusEffect in StatusEffects)
        {
            statusEffect.TickTime(this);
        }
    }
}