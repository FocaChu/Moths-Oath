using MothsOath.Core.Common;
using MothsOath.Core.Common.EffectInterfaces.Combat;
using MothsOath.Core.Common.EffectInterfaces.Death;
using MothsOath.Core.Common.EffectInterfaces.Healing;
using MothsOath.Core.Common.Plans;
using MothsOath.Core.Entities.Archetypes;
using MothsOath.Core.Models.Enums;
using MothsOath.Core.States;
namespace MothsOath.Core.PassiveEffects;

public class RingMasterPassive : BasePassiveEffect, ITurnStartReactor, IModifiedHealthReactor, 
                                                    IKillReactor
{
    public override string Id { get; set; } = "ring_master_passive";

    public override string Name { get; set; } = "Mestre de Pista";

    public override string Description { get; set; } = "Mestre de Pista.";

    public int Priority { get; set; } = 3;


    public void OnHealthModifierApplied(ActionContext context, HealthModifierPlan plan, BaseCharacter target)
    {
        if (plan.ModifierType == HealthModifierType.Healing)
            return;

        var hypeLevel = plan.HasCritical ? plan.FinalValue * .15f : plan.FinalValue * .1f;

        GetNarrator(context.GameState).HypometerLevel += hypeLevel;
    }

    public void OnKill(ActionContext context, MortuaryPlan plan, BaseCharacter victim)
    {
        GetNarrator(context.GameState).HypometerLevel++;

        if(plan.HealthModifierPlan.HasCritical)
            GetNarrator(context.GameState).HypometerLevel++;
    }

    public void OnTurnStart(BaseCharacter target, CombatState context)
    {
        var bannedEffects = target.StatusEffects.Where(se => se.Id == "silence_effect"
                                               || se.Id == "taunted_effect").ToList();

        if (bannedEffects.Count == 0)
            return;

        target.StatusEffects.RemoveAll(se => bannedEffects.Contains(se));

        var frustrationLevel = bannedEffects.Count;

        target.Stats.TemporaryCriticalChance += frustrationLevel;
        target.Stats.TemporaryCriticalChance++;

        var enemies = context.Enemies.Where(e => e.Allegiance != target.Allegiance
                                            && e.Stats.IsAlive).ToList();

        foreach (var enemy in enemies)
        {
            enemy.Stats.TemporaryCriticalChance -= frustrationLevel;
        }

        GetNarrator(context).HypometerLevel++;
    }

    private Narrator GetNarrator(CombatState context)
    {
        if (context.Player is not Narrator narrator)
            throw new InvalidOperationException("O jogador atual não é um Narrador.");

        return narrator;
    }
}
