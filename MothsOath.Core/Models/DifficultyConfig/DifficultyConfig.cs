namespace MothsOath.Core.Models.DifficultyConfig;

public class DifficultyConfig
{
    public string Name { get; set; } = string.Empty;

    public float EnemyHealthMultiplier { get; set; } = 1;

    public float EnemyStrengthMultiplier { get; set; } = 1;

    public float EnemyDefenseMultiplier { get; set; } = 1;

    public short MinEnemyCount { get; set; } = 3;

    public short MaxEnemyCount { get; set; } = 4;
}
