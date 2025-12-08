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
                EnemyDamageMultiplier = 0.9f,
                EnemyDefenseMultiplier = 0.9f,
                MinEnemyCount = 3,
                MaxEnemyCount = 4
            },
            GameDifficulty.Normal => new DifficultyConfig
            {
                Name = "Normal",
                EnemyHealthMultiplier = 1.0f,
                EnemyDamageMultiplier = 1.0f,
                EnemyDefenseMultiplier = 1.0f,
                MinEnemyCount = 3,
                MaxEnemyCount = 5

            },
            GameDifficulty.Hard => new DifficultyConfig
            {
                Name = "Hard",
                EnemyHealthMultiplier = 1.3f,
                EnemyDamageMultiplier = 1.25f,
                EnemyDefenseMultiplier = 1.25f,
                MinEnemyCount = 3,
                MaxEnemyCount = 5
            },
            _ => new DifficultyConfig
            {
                Name = "Normal",
                EnemyHealthMultiplier = 1.0f,
                EnemyDamageMultiplier = 1.0f,
                EnemyDefenseMultiplier = 1.0f,
                MinEnemyCount = 3,
                MaxEnemyCount = 5
            }
        };
    }
}
