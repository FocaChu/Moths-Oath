using MothsOath.Core.StatusEffect;
using MothsOath.Core.StatusEffect.Interfaces;

namespace MothsOath.Core.Common;

public abstract class Character
{
    public Guid Id { get; protected set; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;

    public int MaxHealth { get; set; }

    public int CurrentHealth { get; set; }

    public int BaseStrength { get; set; }

    public int BonusStrength { get; set; } = 0;

    public int BaseResistance { get; set; }

    public int BonusDefense { get; set; } = 0;

    public int Shield { get; set; } = 0;

    public int Regeneration { get; set; } = 0;

    public int TotalStrength => BaseStrength + BonusStrength;

    public int TotalDefense => BaseResistance + BonusDefense;

    public bool IsAlive => CurrentHealth > 0;

    public List<BaseStatusEffect> StatusEffects { get; set; } = new List<BaseStatusEffect>();

    public event Action<Character, int> OnDamageTaken;

    public void TakeDamage(int damage, bool bypass)
    {
        if (damage > 0)
        {
            if(bypass)
            {
                this.CurrentHealth -= damage;
                return;
            }

            damage -= TotalDefense;

            if (this.Shield > 0)
            {
                int absorvedDamage = Math.Min(damage, this.Shield);
                damage -= absorvedDamage;
                this.Shield -= absorvedDamage;
            }

            int finalDamage = Math.Max(damage, 0);

            if (finalDamage > 0)
            {
                CurrentHealth -= finalDamage;
                OnDamageTaken?.Invoke(this, finalDamage); 
            }
        }
    }

    public void Heal(HealPlan plan)
    {
        if(plan.BaseHealAmount <= 0)
            return;

        var healthModifiers = this.StatusEffects.OfType<IHealModifier>().ToList();

        foreach (var effect in healthModifiers)
        {
            effect.ModifierHealthPlan(plan, this);
        }

        if (!plan.CanProceed || plan.FinalHealAmount == 0)
            return;

        CurrentHealth = Math.Min(CurrentHealth + plan.FinalHealAmount, MaxHealth);

        var healthReactors = this.StatusEffects.OfType<IHealReactor>().ToList();

        foreach (var effect in healthReactors)
        {
            effect.ReactToHeal(plan, this);
        }
    }

    public void ApplyStatusEffect(BaseStatusEffect statusEffect)
    {
        if (StatusEffects.Any(se => se.Id == statusEffect.Id))
        {
            var existingEffect = StatusEffects.First(se => se.Id == statusEffect.Id);
            existingEffect.StackEffect(statusEffect);
            return;
        }

        StatusEffects.Add(statusEffect);
    }

    public void TickStatusEffects()
    {
        if(!StatusEffects.Any())
            return;

        foreach (var statusEffect in StatusEffects)
        {
            statusEffect.TickTime(this);
        }

        StatusEffects.RemoveAll(se => se.Duration <= 0);
    }
}