using MothsOath.Core.Common;
using MothsOath.Core.States;

namespace MothsOath.Core.Entities;

public class Player : Character
{
    public string Archetype { get; set; } = null!;

    public int MaxMana { get; set; }

    public int CurrentMana { get; set; }

    public int MaxStamina { get; set; }

    public int CurrentStamina { get; set; }

    public int Gold { get; set; } = 100;

    public float XpMultiplier { get; set; } = 1;

    public int Level { get; set; } = 1;

    public int LevelUpPoints { get; set; } = 0;

    public int Xp { get; set; } = 0;

    public int XpToNextLevel { get; set; } = 100;

    public int CardsByTurn { get; set; } = 5;

    public List<BaseCard> Deck { get; private set; } = new List<BaseCard>();

    public List<BaseCard> Hand { get; private set; } = new List<BaseCard>();

    public List<BaseCard> DiscartPile { get; private set; } = new List<BaseCard>();

    public virtual void OnTurnStart(CombatState state)
    {
        Restore();

        if (CardsByTurn > Deck.Count)
        {
            DrawCards(Deck.Count > 0 ? Deck.Count : 1);
            return;
        }
        
        DrawCards(CardsByTurn);
    }

    public void ShuffleDeck()
    {
        var rnd = new Random();

        Deck = Deck.OrderBy(x => rnd.Next()).ToList();
    }

    public void DrawCards(int numberOfCards)
    {
       for (int i = 0; i < numberOfCards; i++)
        {
            if (Deck.Count == 0)
            {
                Deck.AddRange(DiscartPile);
                DiscartPile.Clear();
                ShuffleDeck();
            }

            if (Deck.Count == 0)
            {
                break;
            }

            var card = Deck[0];
            Deck.Remove(card);
            Hand.Add(card);
        }
    }

    public void PlayCard(ActionContext context)
    {
        if (!CanAfford(context.Card))
        {
            Console.WriteLine($"Cannot afford to play card: {context.Card.Name}");
        }
        else
        {
            PayCosts(context.Card);

            context.Card.PlayEffect(context);

            this.Hand.Remove(context.Card);
            this.DiscartPile.Add(context.Card);
        }
    }

    public bool CanAfford(BaseCard card)
    {
        return Stats.CurrentHealth >= card.HealthCost &&
               this.CurrentStamina >= card.StaminaCost &&
               this.CurrentMana >= card.ManaCost &&
               Gold >= card.GoldCost;
    }

    public void PayCosts(BaseCard card)
    {
        Stats.CurrentHealth -= card.HealthCost;
        
        this.CurrentMana -= card.ManaCost;
        
        this.CurrentStamina -= card.StaminaCost;
        
        this.Gold -= card.GoldCost;
    }

    public override void Restore()
    {
        base.Restore();

        this.CurrentMana = this.MaxMana;
        this.CurrentStamina = this.MaxStamina;
    }

    public void GainXp(int amount)
    {
        Xp += (int)(amount * XpMultiplier);
        while (Xp >= XpToNextLevel)
        {
            Xp -= XpToNextLevel;
            LevelUp();
        }
    }

    private void LevelUp()
    {
        Level++;
        LevelUpPoints++;
        this.Stats.MaxHealth += 5;
        this.Stats.CurrentHealth = this.Stats.MaxHealth;

        XpToNextLevel = (int)(XpToNextLevel * 1.5);
        Console.WriteLine($"Player leveled up to level {Level}!");
    }
}
