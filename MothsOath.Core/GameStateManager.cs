using MothsOath.Core.Common;
using MothsOath.Core.Entities;

namespace MothsOath.Core;

public class GameStateManager
{
    public Player? Player { get; private set; }

    public List<Enemy> Enemies { get; private set; } = new List<Enemy>();

    public void StartNewCombat()
    {
        Player = new Player { Name = "Protagonista", MaxHP = 100, CurrentHP = 100 };

        Enemies = new List<Enemy>
        {
            new Enemy { Name = "Rato do Bastião", MaxHP = 20, CurrentHP = 20 },
            new Enemy { Name = "Cultista Desatento", MaxHP = 35, CurrentHP = 35 }
        };

        Player.Hand.Clear();
        Player.Hand.Add(new StandartCard { Name = "Golpe Simples", Description = "Causa 5 de dano." });
        Player.Hand.Add(new StandartCard { Name = "Golpe Simples", Description = "Causa 5 de dano." });
        Player.Hand.Add(new StandartCard { Name = "Defesa Básica", Description = "Ganha 5 de escudo." });

        Console.WriteLine("New Combat Started!");
    }

    public void PlayCard(BaseCard card, Character target)
    {
        Console.WriteLine($"Jogador jogou a carta '{card.Name}' no alvo '{target.Name}'.");

        if (card.Name == "Golpe Simples")
        {
            target.CurrentHP -= 5;
            if (target.CurrentHP < 0) target.CurrentHP = 0;
        }

        Player?.Hand.Remove(card);
    }
}