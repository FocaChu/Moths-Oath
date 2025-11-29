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

        Console.WriteLine("New Combat Started!");
    }
}