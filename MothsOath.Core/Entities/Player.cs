using MothsOath.Core.Common;
using MothsOath.Core.States;

namespace MothsOath.Core.Entities;

public class Player : Character
{
    public int MaxMana { get; set; }

    public int CurrentMana { get; set; }

    public int MaxStamina { get; set; }

    public int CurrentStamina { get; set; }

    public int Gold { get; set; }

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
            Deck.RemoveAt(0);
            Hand.Add(card);
        }
    }

    public void PlayCard(BaseCard card, Character target, CombatState gameState)
    {
        if (CanAfford(card))
        {
            PayCosts(card);

            card.PlayEffect(this, target, gameState);

            this.Hand.Remove(card);
            this.DiscartPile.Add(card);
        }
        else 
        {
            Console.WriteLine($"Cannot afford to play card: {card.Name}");
        }
    }

    public bool CanAfford(BaseCard card)
    {
        return this.CurrentHealth >= card.HealthCost &&
               this.CurrentStamina >= card.StaminaCost &&
               this.CurrentMana >= card.ManaCost &&
               this.Gold >= card.GoldCost;
    }

    public void PayCosts(BaseCard card)
    {
        this.CurrentHealth -= card.HealthCost;
        
        this.CurrentMana -= card.ManaCost;
        
        this.CurrentStamina -= card.StaminaCost;
        
        this.Gold -= card.GoldCost;
    }
}
