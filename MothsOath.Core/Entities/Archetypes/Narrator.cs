using MothsOath.Core.Abilities;
using MothsOath.Core.Actions;
using MothsOath.Core.Behaviors;
using MothsOath.Core.Common;
using MothsOath.Core.PassiveEffects;
using MothsOath.Core.States;

namespace MothsOath.Core.Entities.Archetypes;

public class Narrator : Player
{
    private const int HighestTier = 4;

    public Narrator(Player player)
    {
        this.Id = player.Id;

        this.Name = player.Name;
        this.Archetype = player.Archetype;
        this.Race = player.Race;
        this.Allegiance = player.Allegiance;

        var s = player.Stats;
        this.Stats = new Stats
        {
            MaxHealth = s.MaxHealth,
            CurrentHealth = s.CurrentHealth,
            BaseStrength = s.BaseStrength,
            BaseKnowledge = s.BaseKnowledge,
            BaseDefense = s.BaseDefense,
            BaseCriticalChance = s.BaseCriticalChance,
            BaseCriticalDamageMultiplier = s.BaseCriticalDamageMultiplier,
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
        if (state.Allies.Count >= 6)
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

        if(currentTier == 0 || currentTier == HighestTier)
            return;

        var cost = CalculatePromotionCost(currentAllies, currentTier);

        if (this.CurrentMana < cost) 
            return;

        if(!PromoteTitle(state, actor, currentTier))
            return;

        this.CurrentMana -= cost;
    }

    public int GetActorTier(BaseCharacter character)
    {
        return character.Name switch
        {
           string name when name.Contains("Figurante") => 1,
           string name when name.Contains("Coadjuvante") => 2,
           string name when name.Contains("Protagonista") => 3,
           string name when name.Contains("Vilão") => 4,
           string name when name.Contains("Herói") => 4,
           string name when name.Contains("Estagiário") => 4,
            _ => 0,
        };
    }

    public bool PromoteTitle(CombatState state, CharacterNPC actor, int tier)
    {
        if (tier == 0 || tier == HighestTier) 
            return false;

        switch (tier)
        {
            case 1:
                actor.Name = "Coadjuvante";

                actor.Stats.MaxHealth += 10;
                actor.ReceivePureHeal(10 + actor.Stats.Regeneration);

                actor.Stats.BaseStrength += 2;
                actor.Stats.BaseKnowledge += 2;
                actor.Stats.BaseDefense += 1;

                actor.SpecialBehavior = new TargetOthersFromSameTeam();
                actor.SpecialAbility = new AssistantAction();
                break;  

            case 2:
                actor.Name = "Protagonista";

                actor.Stats.MaxHealth += 15;
                actor.ReceivePureHeal(15 + actor.Stats.Regeneration);

                actor.Stats.BaseStrength += 2;
                actor.Stats.BaseKnowledge += 2;
                actor.Stats.BaseDefense += 1;

                actor.SpecialBehavior = new TargetAllBehavior();
                actor.SpecialAbility = new BeTheStarAction();
                break;

            case 3:
                if(MegaEvolveActor(state, actor, tier)){
                    return true;
                }
                break;
            default:
                return false;
        }

        return true;
    }

    private bool MegaEvolveActor(CombatState state, CharacterNPC actor, int tier)
    {
        if(tier != 3) 
            return false;

        if(actor.Stats.CurrentHealth < actor.Stats.TotalMaxHealth * 0.5f && actor.CanUseSpecial)
        {
            MegaEvolveVillain(state, actor);
            return true;
        }
        else if(actor.Stats.CurrentHealth >= actor.Stats.TotalMaxHealth * 0.5f && actor.CanUseSpecial)
        {
            MegaEvolveHero(state, actor);
            return true;
        }
        else if(!actor.CanUseSpecial)
        {
            MegaEvolveTrainee(state, actor);
            return true;
        }

        return false;
    }

    private void MegaEvolveVillain(CombatState state, CharacterNPC actor)
    {
        Console.WriteLine("Plot Twist! O protagonista se tornou um vilão!");
        actor.Name = "Vilão";

        actor.Stats.MaxHealth += 15;
        actor.ReceivePureHeal(20 + actor.Stats.Regeneration);

        actor.Stats.BaseStrength += 3;
        actor.Stats.BaseDefense += 1;
        actor.Stats.BaseCriticalChance += 1;

        actor.PassiveEffects.Add(new CleaveDamagePassiveEffect());
        actor.PassiveEffects.Add(new VampirismPassiveEffect());
        actor.SpecialBehavior = new TargetOpposingTeam();
        actor.SpecialAbility = new BeTheVillainAction();
    }

    private void MegaEvolveHero(CombatState state, CharacterNPC actor)
    {
        Console.WriteLine("Climax! O protagonista se tornou um herói!");
        actor.Name = "Herói";

        actor.Stats.MaxHealth += 15;
        actor.ReceivePureHeal(15 + actor.Stats.Regeneration);

        actor.Stats.BaseStrength += 3;
        actor.Stats.BaseDefense += 1;
        actor.Stats.Regeneration += 1;

        actor.PassiveEffects.Add(new VictoriousGloryPassiveEffect());
        actor.SpecialAbility = new BeTheHeroAction();
    }

    private void MegaEvolveTrainee(CombatState state, CharacterNPC actor)
    {
        Console.WriteLine("Mudança de elenco! O protagonista se tornou um estagiário!");
        actor.Name = "Estagiário";

        actor.Stats.MaxHealth += 10;
        actor.ReceivePureHeal(15 + actor.Stats.Regeneration);

        actor.Stats.BaseKnowledge += 2;
        actor.Stats.BaseDefense += 1;

        actor.NormalBehavior = new TargetOthersFromSameTeam();
        actor.BasicAbility = new AssistantAction();

        actor.SpecialBehavior = new TargetOthersFromSameTeam();
        actor.SpecialAbility = new BeTheTraineeAction();
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
