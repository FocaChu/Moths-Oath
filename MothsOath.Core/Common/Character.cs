using MothsOath.Core.Common.EffectInterfaces.Damage;
using MothsOath.Core.Common.EffectInterfaces.Healing;
using MothsOath.Core.Common.EffectInterfaces.StatusEffect;
using MothsOath.Core.Common.Plans;
using MothsOath.Core.PassiveEffects;
using MothsOath.Core.StatusEffect;

namespace MothsOath.Core.Common;

public abstract class Character
{
    public Guid Id { get; protected set; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;

    public Stats Stats { get; set; } = new Stats();

    public List<BasePassiveEffect> PassiveEffects { get; set; } = new List<BasePassiveEffect>();
    public List<BaseStatusEffect> StatusEffects { get; set; } = new List<BaseStatusEffect>();

    public event Action<Character, int> OnDamageTaken;

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
            var incomingModifiers = this.StatusEffects.OfType<IIncomingDamageModifier>().ToList();

            foreach (var modifier in incomingModifiers)
            {
                modifier.ModifyIncomingDamage(context, plan, this);
            }

            var incomingPassiveModifiers = this.PassiveEffects.OfType<IIncomingDamageModifier>().ToList();

            foreach (var modifier in incomingPassiveModifiers)
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
            var damageReactors = this.StatusEffects.OfType<IDamageReceivedReactor>().ToList();

            foreach (var effect in damageReactors)
            {
                effect.OnDamageReceived(context, plan, this);
            }

            var damagePassiveReactors = this.PassiveEffects.OfType<IDamageReceivedReactor>().ToList();

            foreach (var effect in damagePassiveReactors)
            {
                effect.OnDamageReceived(context, plan, this);
            }
        }

        if (!context.CanDealtReactors)
            return;

        var sourceDamageReactors = context.Source.StatusEffects.OfType<IDamageDealtReactor>().ToList();

        foreach (var effect in sourceDamageReactors)
        {
            effect.OnDamageDealt(context, plan, this);
        }

        var sourceDamagePassiveReactors = context.Source.PassiveEffects.OfType<IDamageDealtReactor>().ToList();

        foreach (var effect in sourceDamagePassiveReactors)
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
            var incomingModifiers = this.StatusEffects.OfType<IIncomingHealingModifier>().ToList();

            foreach (var modifier in incomingModifiers)
            {
                modifier.ModifyIncomingHealing(context, plan, this);
            }

            var incomingPassiveModifiers = this.PassiveEffects.OfType<IIncomingHealingModifier>().ToList();

            foreach (var modifier in incomingPassiveModifiers)
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
            var healthReactors = this.StatusEffects.OfType<IHealingReceivedReactor>().ToList();

            foreach (var effect in healthReactors)
            {
                effect.OnHealingReceived(context, plan, this);
            }

            var healthPassiveReactors = this.PassiveEffects.OfType<IHealingReceivedReactor>().ToList();

            foreach (var effect in healthPassiveReactors)
            {
                effect.OnHealingReceived(context, plan, this);
            }
        }

        if (!context.CanDealtReactors)
            return;

        var sourceHealthReactors = context.Source.StatusEffects.OfType<IHealingDoneReactor>().ToList();

        foreach (var effect in sourceHealthReactors)
        {
            effect.OnHealingDone(context, plan, this);
        }

        var sourceHealthPassiveReactors = context.Source.PassiveEffects.OfType<IHealingDoneReactor>().ToList();

        foreach (var effect in sourceHealthPassiveReactors)
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
            var incomingModifiers = this.StatusEffects.OfType<IIncomingStatusEffectModifier>().ToList();

            foreach (var modifier in incomingModifiers)
            {
                modifier.ModifyIncomingStatusEffect(context, plan, this);
            }

            var incomingPassiveModifiers = this.PassiveEffects.OfType<IIncomingStatusEffectModifier>().ToList();

            foreach (var modifier in incomingPassiveModifiers)
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
            var statusEffectReactorsTarget = this.StatusEffects.OfType<IStatusEffectAppliedReactor>().ToList();

            foreach (var reactor in statusEffectReactorsTarget)
            {
                reactor.OnStatusEffectApplied(context, plan, this);
            }

            var statusEffectReactorsSource = this.StatusEffects.OfType<IStatusEffectAppliedReactor>().ToList();

            foreach (var reactor in statusEffectReactorsSource)
            {
                reactor.OnStatusEffectApplied(context, plan, this);
            }
        }

        if (!context.CanRecievedReactors)
            return;

        var statusEffectReactors = context.Source.StatusEffects.OfType<IStatusEffectDoneReactor>().ToList();

        foreach (var reactor in statusEffectReactors)
        {
            reactor.OnStatusEffectDone(context, plan, this);
        }

        var statusEffectPassiveReactors = context.Source.PassiveEffects.OfType<IStatusEffectDoneReactor>().ToList();

        foreach (var reactor in statusEffectPassiveReactors)
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

    public void TickStatusEffects()
    {
        if (!StatusEffects.Any())
            return;

        foreach (var statusEffect in StatusEffects)
        {
            statusEffect.TickTime(this);
        }

        ClearFadingStatusEffects();
    }

    public void ClearFadingStatusEffects()
    {
        StatusEffects.RemoveAll(se => se.Duration <= 0);
    }
}