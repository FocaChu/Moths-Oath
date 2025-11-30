using MothsOath.Core.Common;
using MothsOath.Core.Entities;
using MothsOath.Core.Factories;

namespace MothsOath.Core;

public class GameStateManager
{
    public Player? Player { get; private set; }
    public List<Enemy> Enemies { get; private set; } = new List<Enemy>();

    private readonly EnemyFactory _enemyFactory;

    public event Action OnCombatStarted;
    public event Action OnCardPlayed;

    public GameStateManager(EnemyFactory enemyFactory)
    {
        _enemyFactory = enemyFactory;
    }


    public void StartNewCombat()
    {
        Player = new Player { Name = "Protagonista", MaxHP = 100, CurrentHP = 100 };

        Enemies.Clear();
        Enemies.Add(_enemyFactory.CreateEnemy("skeleton_warrior"));
        Enemies.Add(_enemyFactory.CreateEnemy("skeleton_warrior"));

        Player.Hand.Clear();
        Player.Hand.Add(new StandartCard { Name = "Golpe Simples", Description = "Causa 5 de dano." });
        Player.Hand.Add(new StandartCard { Name = "Defesa Básica", Description = "Ganha 5 de escudo." });

        Console.WriteLine("New Combat Started!");
        OnCombatStarted?.Invoke(); 
    }

    public void PlayCard(BaseCard card, Character target)
    {
        if (Player == null) return; 

        Console.WriteLine($"Jogador jogou a carta '{card.Name}' no alvo '{target.Name}'.");

        if (card.Name == "Golpe Simples")
        {
            target.TakeDamage(5, false);
        }

        Player.Hand.Remove(card);

        OnCardPlayed?.Invoke();
    }
}