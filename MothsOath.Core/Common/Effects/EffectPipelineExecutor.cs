using MothsOath.Core.Common.EffectInterfaces;
using MothsOath.Core.Common.EffectInterfaces.Combat;
using MothsOath.Core.Common.EffectInterfaces.Death;
using MothsOath.Core.Common.EffectInterfaces.Health;
using MothsOath.Core.Common.EffectInterfaces.StatusEffect;
using MothsOath.Core.Common.Plans;
using MothsOath.Core.States;

namespace MothsOath.Core.Common.Effects;

/// <summary>
/// Centralized executor for all effect pipelines.
/// Handles querying, filtering, and executing reactors in correct order.
/// Provides single point of execution for all effect types, improving maintainability and observability.
/// </summary>
public static class EffectPipelineExecutor
{
    #region Health Modifiers

    /// <summary>
    /// Executes incoming health modifiers on target (before damage/heal is applied).
    /// This allows effects to modify damage/healing before it's calculated.
    /// </summary>
    public static void ExecuteIncomingHealthModifiers(
        BaseCharacter target,
        ActionContext context,
        HealthModifierPlan plan)
    {
        if (!context.CanIncomingModifiers)
            return;

        var modifiers = target.GetEffectCache()
            .GetReactors<IIncomingHealthModifierReactor>();

        foreach (var modifier in EffectQueryService.FilterActive(modifiers))
        {
            modifier.ModifyIncomingHealthModifier(context, plan, target);

            if (!plan.CanProceed)
                return;
        }
    }

    /// <summary>
    /// Executes received health reactors on target (after damage/heal is applied to target).
    /// This allows effects to react to damage/healing that was received.
    /// </summary>
    public static void ExecuteReceivedHealthReactors(
        BaseCharacter target,
        ActionContext context,
        HealthModifierPlan plan)
    {
        if (!context.CanRecievedReactors)
            return;

        var reactors = target.GetEffectCache()
            .GetReactors<IHealthModifierReactor>();

        foreach (var reactor in EffectQueryService.FilterActive(reactors))
        {
            reactor.ReactHealthModified(context, plan, target);
        }
    }

    /// <summary>
    /// Executes dealt health reactors on source (after source causes damage/heal).
    /// This allows the attacker/healer to react to damage/healing they dealt.
    /// </summary>
    public static void ExecuteDealtHealthReactors(
        BaseCharacter source,
        ActionContext context,
        HealthModifierPlan plan,
        BaseCharacter target)
    {
        if (!context.CanDealtReactors)
            return;

        var reactors = source.GetEffectCache()
            .GetReactors<IModifiedHealthReactor>();

        foreach (var reactor in EffectQueryService.FilterActive(reactors))
        {
            reactor.OnHealthModifierApplied(context, plan, target);
        }
    }

    #endregion

    #region Status Effects

    /// <summary>
    /// Executes incoming status effect modifiers on target (before status effect is applied).
    /// This allows effects to modify or block incoming status effects.
    /// </summary>
    public static void ExecuteIncomingStatusEffectModifiers(
        BaseCharacter target,
        ActionContext context,
        StatusEffectPlan plan)
    {
        if (!context.CanIncomingModifiers)
            return;

        var modifiers = target.GetEffectCache()
            .GetReactors<IIncomingStatusEffectModifier>();

        foreach (var modifier in EffectQueryService.FilterActive(modifiers))
        {
            modifier.ModifyIncomingStatusEffect(context, plan, target);

            if (!context.CanProceed)
                return;
        }
    }

    /// <summary>
    /// Executes status effect applied reactors on target (after status effect is applied to target).
    /// This allows target's effects to react to receiving a status effect.
    /// </summary>
    public static void ExecuteStatusEffectAppliedReactors(
        BaseCharacter target,
        ActionContext context,
        StatusEffectPlan plan)
    {
        if (!context.CanRecievedReactors)
            return;

        var reactors = target.GetEffectCache()
            .GetReactors<IStatusEffectAppliedReactor>();

        foreach (var reactor in EffectQueryService.FilterActive(reactors))
        {
            reactor.OnStatusEffectApplied(context, plan, target);
        }
    }

    /// <summary>
    /// Executes status effect done reactors on source (after source applies status effect).
    /// This allows the source to react to successfully applying a status effect.
    /// </summary>
    public static void ExecuteStatusEffectDoneReactors(
        BaseCharacter source,
        ActionContext context,
        StatusEffectPlan plan,
        BaseCharacter target)
    {
        if (!context.CanRecievedReactors)
            return;

        var reactors = source.GetEffectCache()
            .GetReactors<IStatusEffectDoneReactor>();

        foreach (var reactor in EffectQueryService.FilterActive(reactors))
        {
            reactor.OnStatusEffectDone(context, plan, target);
        }
    }

    #endregion

    #region Combat Lifecycle

    /// <summary>
    /// Executes turn start effects on a character.
    /// Called at the beginning of a character's turn.
    /// </summary>
    public static void ExecuteTurnStartEffects(
        BaseCharacter character,
        CombatState combatState)
    {
        var reactors = character.GetEffectCache()
            .GetReactors<ITurnStartReactor>();

        foreach (var reactor in EffectQueryService.FilterActive(reactors))
        {
            reactor.OnTurnStart(character, combatState);
        }
    }

    /// <summary>
    /// Executes turn end effects on a character.
    /// Called at the end of a character's turn.
    /// </summary>
    public static void ExecuteTurnEndEffects(
        BaseCharacter character,
        CombatState combatState)
    {
        var reactors = character.GetEffectCache()
            .GetReactors<ITurnEndReactor>();

        foreach (var reactor in EffectQueryService.FilterActive(reactors))
        {
            reactor.OnTurnEnd(character, combatState);
        }
    }

    /// <summary>
    /// Executes combat start effects on a character.
    /// Called when combat begins.
    /// </summary>
    public static void ExecuteCombatStartEffects(
        BaseCharacter character,
        CombatState combatState)
    {
        var reactors = character.GetEffectCache()
            .GetReactors<ICombatStartReactor>();

        foreach (var reactor in EffectQueryService.FilterActive(reactors))
        {
            reactor.OnCombatStart(character, combatState);
        }
    }

    /// <summary>
    /// Executes combat end effects on a character.
    /// Called when combat ends.
    /// </summary>
    public static void ExecuteCombatEndEffects(
        BaseCharacter character,
        CombatState combatState)
    {
        var reactors = character.GetEffectCache()
            .GetReactors<ICombatEndReactor>();

        foreach (var reactor in EffectQueryService.FilterActive(reactors))
        {
            reactor.OnCombatEnd(combatState, character);
        }
    }

    #endregion

    #region Death

    /// <summary>
    /// Executes death effects when a character dies.
    /// First executes the victim's death reactors, then the killer's kill reactors.
    /// </summary>
    public static void ExecuteDeathEffects(
        BaseCharacter victim,
        ActionContext context,
        MortuaryPlan plan)
    {
        if (!context.CanDeathReactors)
            return;

        // Victim's death reactors
        var deathReactors = victim.GetEffectCache()
            .GetReactors<IDeathReactor>();

        foreach (var reactor in EffectQueryService.FilterActive(deathReactors))
        {
            reactor.OnDeath(context, plan, victim);
        }

        // Source's kill reactors
        var killReactors = context.Source.GetEffectCache()
            .GetReactors<IKillReactor>();

        foreach (var reactor in EffectQueryService.FilterActive(killReactors))
        {
            reactor.OnKill(context, plan, victim);
        }
    }

    #endregion

    #region Other

    /// <summary>
    /// Executes action plan modifiers on source.
    /// Called before an action is executed to allow modification of the action plan.
    /// </summary>
    public static void ExecuteActionPlanModifiers(
        BaseCharacter source,
        ActionContext context)
    {
        var modifiers = source.GetEffectCache()
            .GetReactors<IActionPlanModifier>();

        foreach (var modifier in EffectQueryService.FilterActive(modifiers))
        {
            modifier.ModifyActionPlan(context);
        }
    }

    /// <summary>
    /// Executes fading reactors on a character.
    /// Called when status effects expire/fade.
    /// </summary>
    public static void ExecuteFadingEffects(
        BaseCharacter character,
        CombatState combatState,
        IEnumerable<StatusEffect.BaseStatusEffect> fadedEffects)
    {
        var reactors = fadedEffects.OfType<IFadingReactor>()
            .OrderByDescending(r => r.Priority);

        foreach (var reactor in reactors)
        {
            reactor.OnFading(character, combatState);
        }
    }

    #endregion
}
