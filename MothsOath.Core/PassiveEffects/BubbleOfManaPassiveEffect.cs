using MothsOath.Core.Common;
using MothsOath.Core.Common.EffectInterfaces.Combat;
using MothsOath.Core.Common.EffectInterfaces.Death;
using MothsOath.Core.Common.EffectInterfaces.StatusEffect;
using MothsOath.Core.Common.Plans;
using MothsOath.Core.States;

namespace MothsOath.Core.PassiveEffects;

public class BubbleOfManaPassiveEffect : BasePassiveEffect, IDeathReactor, ITurnStartReactor, IIncomingStatusEffectModifier
{
    public override string Id { get; set; } = "bubble_of_mana_passive";

    public override string Name { get; set; } = "Bolha de Mana";

    public int Priority { get; set; } = 1;

    public override string Description { get; set; } = "Restaura uma quantidade de mana do Personagem ao sair de cena.";

    public void ModifyIncomingStatusEffect(ActionContext context, StatusEffectPlan plan, BaseCharacter target)
    {
        if (plan.StatusEffect.Id != "silence_effect" && plan.StatusEffect.Id != "taunted_effect")
            return;

        context.CanProceed = false;

        target.Stats.TemporaryCriticalChance++;
        target.Stats.BonusCriticalChance++;

        Console.WriteLine($"{context.Source.Name} retalia em frustração contra {target.Name}!");

        context.Source.ReceivePureDamage((int)(plan.StatusEffect.Level * target.Stats.TotalCriticalDamageMultiplier));
    }

    public void OnDeath(ActionContext context, MortuaryPlan plan, BaseCharacter victim)
    {
        var player = context.GameState.Player;
        int manaRestored = 10;
        player.CurrentMana = Math.Min(player.CurrentMana + manaRestored, player.MaxMana);
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
        target.Stats.BonusCriticalChance++;

        Console.WriteLine($"{target.Name} pissoteia o chão em frustração!");
    }
}
