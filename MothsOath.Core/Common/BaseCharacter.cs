using MothsOath.Core.Common.EffectInterfaces;
using MothsOath.Core.Common.EffectInterfaces.Damage;
using MothsOath.Core.Common.EffectInterfaces.Healing;
using MothsOath.Core.Common.EffectInterfaces.StatusEffect;
using MothsOath.Core.Common.EffectInterfaces.Turn;
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

    public Allegiance Allegiance { get; set; }

    public List<BasePassiveEffect> PassiveEffects { get; set; } = new List<BasePassiveEffect>();
    public List<BaseStatusEffect> StatusEffects { get; set; } = new List<BaseStatusEffect>();

    public event Action<BaseCharacter, int> OnDamageTaken;

    public virtual void Restore()
    {
        Stats.CurrentHealth = Math.Min(Stats.CurrentHealth + Stats.Regeneration, Stats.MaxHealth);

        this.Stats.BonusStrength = 0;
        this.Stats.BonusKnowledge = 0;
        this.Stats.BonusDefense = 0;
    }

    public void RecievePureDamage(int amount)
    {
        if (amount <= 0)
            return;
        Stats.CurrentHealth -= amount;
        OnDamageTaken?.Invoke(this, amount);
    }

    public void RecieveDamage(ActionContext context, DamagePlan plan)
    {
        if (context.CanIncomingModifiers)
        {
            var incomingModifiers = this.StatusEffects.OfType<IIncomingDamageModifier>().ToList()
                .Concat(this.PassiveEffects.OfType<IIncomingDamageModifier>().ToList())
                .OrderByDescending(m => m.Priority);

            foreach (var modifier in incomingModifiers)
            {
                modifier.ModifyIncomingDamage(context, plan, this);
            }
        }

        plan.FinalDamageAmount = CalculateDamageAmount(plan.FinalDamageAmount, plan.BypassResistance);

        if (!plan.CanProceed || plan.FinalDamageAmount == 0)
            return;

        Stats.CurrentHealth -= plan.FinalDamageAmount;

        if (context.CanRecievedReactors)
        {
            var damageReactors = this.StatusEffects.OfType<IDamageReceivedReactor>().ToList()
                .Concat(this.PassiveEffects.OfType<IDamageReceivedReactor>().ToList())
                .OrderByDescending(m => m.Priority);

            foreach (var effect in damageReactors)
            {
                effect.OnDamageReceived(context, plan, this);
            }
        }

        if (!context.CanDealtReactors)
            return;

        var sourceDamageReactors = context.Source.StatusEffects.OfType<IDamageDealtReactor>().ToList()
            .Concat(context.Source.PassiveEffects.OfType<IDamageDealtReactor>().ToList())
            .OrderByDescending(m => m.Priority);

        foreach (var effect in sourceDamageReactors)
        {
            effect.OnDamageDealt(context, plan, this);
        }
    }

    public void RecievePureHeal(int amount)
    {
        if (amount <= 0)
            return;
        Stats.CurrentHealth = Math.Min(Stats.CurrentHealth + amount, Stats.MaxHealth);
    }

    public void RecieveHeal(ActionContext context, HealPlan plan)
    {
        if (context.CanIncomingModifiers)
        {
            var incomingModifiers = this.StatusEffects.OfType<IIncomingHealingModifier>().ToList()
                .Concat(this.PassiveEffects.OfType<IIncomingHealingModifier>().ToList())
                .OrderByDescending(m => m.Priority);

            foreach (var modifier in incomingModifiers)
            {
                modifier.ModifyIncomingHealing(context, plan, this);
            }
        }

        if (!plan.CanProceed || plan.FinalHealAmount == 0)
            return;

        int healthBefore = Stats.CurrentHealth;

        Stats.CurrentHealth = Math.Min(Stats.CurrentHealth + plan.FinalHealAmount, Stats.MaxHealth);

        int actualHealedAmount = Stats.CurrentHealth - healthBefore;
        plan.FinalHealAmount = actualHealedAmount;

        if (context.CanRecievedReactors)
        {
            var healthReactors = this.StatusEffects.OfType<IHealingReceivedReactor>().ToList()
                .Concat(this.PassiveEffects.OfType<IHealingReceivedReactor>().ToList())
                .OrderByDescending(m => m.Priority);

            foreach (var effect in healthReactors)
            {
                effect.OnHealingReceived(context, plan, this);
            }
        }

        if (!context.CanDealtReactors)
            return;

        var sourceHealthReactors = context.Source.StatusEffects.OfType<IHealingDoneReactor>().ToList()
            .Concat(context.Source.PassiveEffects.OfType<IHealingDoneReactor>().ToList())
            .OrderByDescending(m => m.Priority);

        foreach (var effect in sourceHealthReactors)
        {
            effect.OnHealingDone(context, plan, this);
        }
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

    private int CalculateDamageAmount(int baseAmount, bool bypassResistance)
    {
        if (baseAmount > 0)
        {
            if (bypassResistance)
            {
                return baseAmount;
            }

            baseAmount -= Stats.TotalDefense;

            if (Stats.Shield > 0)
            {
                int absorvedDamage = Math.Min(baseAmount, Stats.Shield);
                baseAmount -= absorvedDamage;
                Stats.Shield -= absorvedDamage;
            }

            int finalDamage = Math.Max(baseAmount, 0);

            return finalDamage;
        }
        return 0;
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