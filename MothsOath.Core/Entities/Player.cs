using MothsOath.Core.Common;
using MothsOath.Core.States;

namespace MothsOath.Core.Entities;

public class Player : Character
{
    public int MaxMana { get; set; }

    public int CurrentMana { get; set; }

    public int MaxStamina { get; set; }

    public int CurrentStamina { get; set; }

    public int Gold { get; set; } = 100;

    public float XpMultiplier { get; set; } = 1;

    public int CardsByTurn { get; set; } = 5;

    public List<BaseCard> Deck { get; private set; } = new List<BaseCard>();

    public List<BaseCard> Hand { get; private set; } = new List<BaseCard>();

    public List<BaseCard> DiscartPile { get; private set; } = new List<BaseCard>();

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

    public void Restore()
    {
        Stats.CurrentHealth = Math.Min(Stats.CurrentHealth + Stats.Regeneration, Stats.MaxHealth);

        this.CurrentMana = this.MaxMana;
        this.CurrentStamina = this.MaxStamina;
    }
}
