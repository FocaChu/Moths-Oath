using MothsOath.Core.Common;
using MothsOath.Core.Common.EffectInterfaces.Combat;
using MothsOath.Core.Common.EffectInterfaces.Damage;
using MothsOath.Core.Common.EffectInterfaces.Healing;
using MothsOath.Core.Common.Plans;
using MothsOath.Core.Entities;
using MothsOath.Core.States;

namespace MothsOath.Core.PassiveEffects;

public class BloodKnotPassiveEffect : BasePassiveEffect, IDamageReceivedReactor, IHealingReceivedReactor, ITurnStartReactor
{
    public override string Id { get; set; } = "blood_knot_passive";

    public override string Name { get; set; } = "Laço de Sangue";

    public override string Description { get; set; } = "Ao atingir vida critica entre em estado de sede de sangue.";

    public int Priority { get; set; } = 2;

    public bool HasActivated { get; set; } = false;

    public void OnDamageReceived(ActionContext context, HealthModifierPlan plan, BaseCharacter target)
    {
        ExecuteEffect(target);
    }

    public void OnTurnStart(BaseCharacter target, CombatState context)
    {
        ExecuteEffect(target);
    }

    public void OnHealingReceived(ActionContext context, HealthModifierPlan plan, BaseCharacter target)
    {
        ExecuteEffect(target);
    }

    private void ExecuteEffect(BaseCharacter target)
    {
        if (IsLowHealth(target) && !HasActivated)
        {
            HasActivated = true;
            ApplyStats(target);
            ApplyBloodFrenzyEffect(target);
        }
        else if (!IsLowHealth(target) && HasActivated)
        {
            HasActivated = false;
            ResetStats(target);
        }
    }

    private bool IsLowHealth(BaseCharacter target)
    {
        return target.Stats.CurrentHealth <= target.Stats.TotalMaxHealth * 0.33f;
    }

    private void ApplyStats(BaseCharacter target)
    {
        if (HasActivated)
        {
            if (target is Player ghoul)
            {
                ghoul.Stats.BonusStrength += ghoul.Level;
                ghoul.Stats.BonusKnowledge += ghoul.Level;
                ghoul.Stats.BonusDefense += 1;
            }
            else
            {
                target.Stats.BonusStrength += 3;
                target.Stats.BonusKnowledge += 3;
                target.Stats.BonusDefense += 1;
            }
        }
    }

    private void ResetStats(BaseCharacter target)
    {
        if (!HasActivated)
        {
            if (target is Player ghoul)
            {
                ghoul.Stats.BonusStrength -= ghoul.Level;
                ghoul.Stats.BonusKnowledge -= ghoul.Level;
                ghoul.Stats.BonusDefense -= 1;
            }
            else
            {
                target.Stats.BonusStrength -= 3;
                target.Stats.BonusKnowledge -= 3;
                target.Stats.BonusDefense -= 1;
            }
        }
    }

    private void ApplyBloodFrenzyEffect(BaseCharacter target)
    {
        if(target is Player ghoul)
        {
            var level = ghoul.Level;
            var bloodFrenzy = new StatusEffect.ConcreteEffects.BloodFrenzyEffect(level, 1);
            target.ApplyPureStatusEffect(bloodFrenzy);
        }
        else
        {
            var bloodFrenzy = new StatusEffect.ConcreteEffects.BloodFrenzyEffect(3, 1);
            target.ApplyPureStatusEffect(bloodFrenzy);
        }

    }
}
