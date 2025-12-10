using MothsOath.Core.Abilities;
using MothsOath.Core.Common;
using MothsOath.Core.States;

namespace MothsOath.Core.Entities.Archetypes;

public class Narrator : Player
{
    public Narrator(Player player)
    {
        this.Id = player.Id;

        this.Name = player.Name;
        this.Archetype = player.Archetype;
        this.Allegiance = player.Allegiance;

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

    public void CallToStageAbility(CombatState state)
    {
        if (state.Allies.Count >= 5)
            return;

        var cost = CalculateExtraCost(state.Allies.Count);

        if (this.CurrentMana < cost)
            return;

        this.CurrentMana -= cost;

        Console.WriteLine("The Narrator steps onto the stage, commanding attention with a booming voice.");
        
        var newExtra = state.CreateNpc("narrator_extra");

        state.Allies.Add(newExtra);
    }

    public int CalculateExtraCost(int currentAllies)
    {
        const int baseCost = 35;
        const double growthPerAlly = 1.35;

        currentAllies = Math.Clamp(currentAllies, 0, 5);

        double cost = baseCost * Math.Pow(growthPerAlly, currentAllies);

        return (int)Math.Round(cost);
    }

    public void PromoteActorAbility(CombatState state, BaseCharacter character)
    {
        if (character == null || !character.Stats.IsAlive || character is not CharacterNPC actor)
            return;

        var currentAllies = state.Allies.Count;
        var currentTier = GetActorTier(character);

        if(currentTier == 0 || currentTier == 3)
            return;

        var cost = CalculatePromotionCost(currentAllies, currentTier);

        if (this.CurrentMana < cost) 
            return;

        if(!PromoteTitle(actor, currentTier))
            return;

        this.CurrentMana -= cost;
    }

    public int GetActorTier(BaseCharacter character)
    {
        return character.Name switch
        {
           string name when name.Contains("Figurante") => 1,
           string name when name.Contains("Coadjuvante") => 2,
            _ => 0,
        };
    }

    public bool PromoteTitle(CharacterNPC actor, int tier)
    {
        if (tier == 0 || tier == 3) 
            return false;

        switch (tier)
        {
            case 1:
                actor.Name = "Coadjuvante";

                actor.Stats.MaxHealth += 10;
                actor.RecievePureHeal(10 + actor.Stats.Regeneration);

                actor.Stats.BaseStrength += 2;
                actor.Stats.BaseKnowledge += 2;
                actor.Stats.BaseDefense += 1;

                actor.SpecialAbility = new PowerStrikeAction();
                break;  

            case 2:
                actor.Name = "Protagonista";

                actor.Stats.MaxHealth += 15;
                actor.RecievePureHeal(15 + actor.Stats.Regeneration);

                actor.Stats.BaseStrength += 2;
                actor.Stats.BaseKnowledge += 2;
                actor.Stats.BaseDefense += 1;

                break;
            default:
                return false;
        }

        return true;
    }

    public int CalculatePromotionCost(int currentAllies, int currentTier)
    {
        const int baseCost = 25;
        const double allyGrowth = 1.25;
        const double tierGrowth = 1.4;

        currentAllies = Math.Clamp(currentAllies, 0, 5);
        currentTier = Math.Max(0, currentTier);

        double cost =
            baseCost *
            Math.Pow(allyGrowth, currentAllies) *
            Math.Pow(tierGrowth, currentTier);

        return (int)Math.Round(cost);
    }
}
