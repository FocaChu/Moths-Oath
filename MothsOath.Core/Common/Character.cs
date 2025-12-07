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

    public void RecievePureDamage(int amount)
    {
        if (amount <= 0)
            return;
        Stats.CurrentHealth -= amount;
        OnDamageTaken?.Invoke(this, amount);
    }

    public void RecieveDamage(ActionContext context, DamagePlan plan)
    {
        var incomingModifiers = this.StatusEffects.OfType<IIncomingDamageModifier>().ToList();

        foreach (var modifier in incomingModifiers)
        {
            modifier.ModifyIncomingDamage(context, plan, this);
        }

        plan.FinalDamageAmount = CalculateDamageAmount(plan.FinalDamageAmount, plan.BypassResistance);

        if (plan.BaseDamageAmount <= 0 || !plan.CanProceed || plan.FinalDamageAmount == 0)
            return;

        Stats.CurrentHealth -= plan.FinalDamageAmount;

        if (context.CanReactTarget)
        {
            var damageReactors = this.StatusEffects.OfType<IDamageReceivedReactor>().ToList();

            foreach (var effect in damageReactors)
            {
                effect.OnDamageReceived(context, plan, this);
            }
        }

        if (!context.CanReactSource)
            return;

        var sourceDamageReactors = context.Source.StatusEffects.OfType<IDamageDealtReactor>().ToList();

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
        if (plan.BaseHealAmount <= 0 || !plan.CanProceed || plan.FinalHealAmount == 0)
            return;

        var incomingModifiers = this.StatusEffects.OfType<IIncomingHealingModifier>().ToList();

        foreach (var modifier in incomingModifiers)
        {
            modifier.ModifyIncomingHealing(context, plan, this);
        }

        int healthBefore = Stats.CurrentHealth;

        Stats.CurrentHealth = Math.Min(Stats.CurrentHealth + plan.FinalHealAmount, Stats.MaxHealth);

        int actualHealedAmount = Stats.CurrentHealth - healthBefore;
        plan.FinalHealAmount = actualHealedAmount;

        if (context.CanReactTarget)
        {
            var healthReactors = this.StatusEffects.OfType<IHealingReceivedReactor>().ToList();

            foreach (var effect in healthReactors)
            {
                effect.OnHealingReceived(context, plan, this);
            }
        }

        if (!context.CanReactSource)
            return;

        var sourceHealthReactors = context.Source.StatusEffects.OfType<IHealingDoneReactor>().ToList();

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
        var incomingModifiers = this.StatusEffects.OfType<IIncomingStatusEffectModifier>().ToList();

        foreach (var modifier in incomingModifiers)
        {
            modifier.ModifyIncomingStatusEffect(context, plan, this);
        }

        if (!plan.CanApply)
            return;

        if (StatusEffects.Any(se => se.Id == plan.StatusEffect.Id))
        {
            var existingEffect = StatusEffects.First(se => se.Id == plan.StatusEffect.Id);
            existingEffect.StackEffect(this, plan.StatusEffect);
            return;
        }

        if (plan.StatusEffect.Level <= 0 || plan.StatusEffect.Duration <= 0)
            return;

        StatusEffects.Add(plan.StatusEffect);

        if (context.CanReactTarget)
        {
            var statusEffectReactorsTarget = this.StatusEffects.OfType<IStatusEffectRecievedReactor>().ToList();

            foreach (var reactor in statusEffectReactorsTarget)
            {
                reactor.OnStatusEffectReceived(context, plan, this);
            }
        }

        if (!context.CanReactTarget)
            return;

        var statusEffectReactors = this.StatusEffects.OfType<IStatusEffectDoneReactor>().ToList();

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

            baseAmount -= Stats.TotalResistance;

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

        StatusEffects.RemoveAll(se => se.Duration <= 0);
    }
}