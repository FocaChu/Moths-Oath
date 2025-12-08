using MothsOath.Core.Common;
using MothsOath.Core.States;
using MothsOath.Core.StatusEffect;
using MothsOath.Core.StatusEffect.StatusEffectsRandomGenerators;

namespace MothsOath.Core.Entities.Archetypes;

public class BellRinger : Player
{
    public BellRinger(Player player)
    {
        this.Id = player.Id;

        this.Name = player.Name;
        this.Archetype = player.Archetype;

        var s = player.Stats;
        this.Stats = new Stats
        {
            MaxHealth = s.MaxHealth,
            CurrentHealth = s.CurrentHealth,
            BaseStrength = s.BaseStrength,
            BonusStrength = s.BonusStrength,
            BaseKnowledge = s.BaseKnowledge,
            BonusKnowledge = s.BonusKnowledge,
            BaseDefense = s.BaseDefense,
            BonusDefense = s.BonusDefense,
            Shield = s.Shield,
            Regeneration = s.Regeneration
        };

        this.MaxMana = player.MaxMana;
        this.CurrentMana = player.CurrentMana;
        this.MaxStamina = player.MaxStamina;
        this.CurrentStamina = player.CurrentStamina;
        this.Gold = player.Gold;
        this.XpMultiplier = player.XpMultiplier;
        this.CardsByTurn = player.CardsByTurn;

        this.PassiveEffects = player.PassiveEffects.ToList();
        this.StatusEffects = player.StatusEffects.ToList();

        this.Deck.Clear();
        this.Deck.AddRange(player.Deck);

        this.Hand.Clear();
        this.Hand.AddRange(player.Hand);

        this.DiscartPile.Clear();
        this.DiscartPile.AddRange(player.DiscartPile);
    }

    public override void OnTurnStart(CombatState state)
    {
        base.OnTurnStart(state);


        Console.WriteLine("Sou um sineiro.");
    }

    public void BellRingAbility(CombatState state, Character target)
    {
        var effectLevel = (int)(this.Stats.TotalKnowledge / 2) >= 1 ? (int)(this.Stats.TotalKnowledge / 2) : 1;

        if(target != null && target is Enemy)
        {
            var index = state.Enemies.IndexOf((Enemy)target);
            MisfortuneBell(index, effectLevel, state);
        }

        Console.WriteLine("Ding Dong!");
    }

    public void MisfortuneBell(int index, int effectLevel, CombatState state)
    {
        var misfortune = RandomMisfortuneGenerator.GenerateRandomMisfortune(effectLevel, 2);
        var echo = new EchoStatusEffect(misfortune);

        state.Enemies[index].ApplyPureStatusEffect(misfortune);


        if (index > 0)
        {
            var previousEnemy = state.Enemies[index - 1];
            previousEnemy.ApplyPureStatusEffect(echo);
        }

        if (index < state.Enemies.Count - 1)
        {
            var nextEnemy = state.Enemies[index + 1];
            nextEnemy.ApplyPureStatusEffect(echo);
        }
    }

    //public void GloryBell(int index, int effectLevel, CombatState state)
    //{
    //    var glory = RandomGloryGenerator.GenerateRandomGlory(effectLevel, 2);
    //    var echo = new EchoStatusEffect(glory);

    //    state.Allies[index].ApplyPureStatusEffect(glory);

    //    if (index > 0)
    //    {
    //        var previousAlly = state.Allies[index - 1];
    //        previousAlly.ApplyPureStatusEffect(echo);
    //    }

    //    if (index < state.Allies.Count - 1)
    //    {
    //        var nextAlly = state.Allies[index + 1];
    //        nextAlly.ApplyPureStatusEffect(echo);
    //    }
    //}
}
