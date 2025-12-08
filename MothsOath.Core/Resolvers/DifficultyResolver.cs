using MothsOath.Core.Models.DifficultyConfig;
using MothsOath.Core.Models.Enums;

namespace MothsOath.Core.Resolvers;

public static class DifficultyResolver
{
    public static DifficultyConfig ResolveDifficultyConfig(GameDifficulty difficulty)
    {
        return difficulty switch
        {
            GameDifficulty.Easy => new DifficultyConfig
            {
                Name = "Easy",
                EnemyHealthMultiplier = 0.85f,
                EnemyStrengthMultiplier = 0.9f,
                EnemyDefenseMultiplier = 0.9f,
                MinEnemyCount = 3,
                MaxEnemyCount = 3
            },
            GameDifficulty.Normal => new DifficultyConfig
            {
                Name = "Normal",
                EnemyHealthMultiplier = 1.0f,
                EnemyStrengthMultiplier = 1.0f,
                EnemyDefenseMultiplier = 1.0f,
                MinEnemyCount = 3,
                MaxEnemyCount = 4

            },
            GameDifficulty.Hard => new DifficultyConfig
            {
                Name = "Hard",
                EnemyHealthMultiplier = 1.3f,
                EnemyStrengthMultiplier = 1.25f,
                EnemyDefenseMultiplier = 1.25f,
                MinEnemyCount = 3,
                MaxEnemyCount = 5
            },
            _ => new DifficultyConfig
            {
                Name = "Normal",
                EnemyHealthMultiplier = 1.0f,
                EnemyStrengthMultiplier = 1.0f,
                EnemyDefenseMultiplier = 1.0f,
                MinEnemyCount = 3,
                MaxEnemyCount = 5
            }
        };
    }
}
