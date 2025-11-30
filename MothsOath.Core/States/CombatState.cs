using MothsOath.Core.Common;
using MothsOath.Core.Entities;
using MothsOath.Core.Factories;

namespace MothsOath.Core.States;

public class CombatState : IGameState
{
    private readonly GameManager _gameManager;
    private readonly EnemyFactory _enemyFactory;

    public Player Player { get; private set; }
    public List<Enemy> Enemies { get; private set; } = new List<Enemy>();

    public CombatState(GameManager gameManager, EnemyFactory enemyFactory, Player player)
    {
        _gameManager = gameManager;
        _enemyFactory = enemyFactory;
        Player = player; 
    }


    public void OnEnter()
    {
        Player = new Player { Name = "Protagonista", MaxHP = 100, CurrentHP = 100 };

        Enemies.Clear();
        Enemies.Add(_enemyFactory.CreateEnemy("skeleton_warrior"));
        Enemies.Add(_enemyFactory.CreateEnemy("skeleton_warrior"));

        Player.Hand.Clear();
        Player.Hand.Add(new StandartCard { Name = "Golpe Simples", Description = "Causa 5 de dano." });
        Player.Hand.Add(new StandartCard { Name = "Defesa Básica", Description = "Ganha 5 de escudo." });

        Console.WriteLine("New Combat Started!");
    }

    public void OnExit()
    {
        Console.WriteLine("Saindo do estado de combate.");
    }

    public void Update() { }

    public void PlayCard(BaseCard card, Character target)
    {
        if (Player == null) return; 

        Console.WriteLine($"Jogador jogou a carta '{card.Name}' no alvo '{target.Name}'.");

        if (card.Name == "Golpe Simples")
        {
            target.TakeDamage(5, false);
        }

        Player.Hand.Remove(card);

        if (Enemies.All(e => !e.IsAlive))
        {
            //_gameManager.TransitionToState(new MapState(_gameManager, Player));
        }
    }
}