namespace MothsOath.Core.Common;

public class Stats
{
    public int MaxHealth { get; set; }
    public int BonusMaxHealth { get; set; } = 0;
    public int TemporaryMaxHealth { get; set; } = 0;
    public int CurrentHealth { get; set; } = 0;
    public int TotalMaxHealth => MaxHealth + BonusMaxHealth + TemporaryMaxHealth;
    public bool IsAlive => CurrentHealth > 0;


    public int BaseStrength { get; set; }
    public int BonusStrength { get; set; } = 0;
    public int TemporaryStrength { get; set; } = 0;
    public int TotalStrength => BaseStrength + BonusStrength + TemporaryStrength;


    public int BaseKnowledge { get; set; }
    public int BonusKnowledge { get; set; } = 0;
    public int TemporaryKnowledge { get; set; } = 0;
    public int TotalKnowledge => BaseKnowledge + BonusKnowledge + TemporaryKnowledge;


    public int BaseDefense { get; set; }
    public int BonusDefense { get; set; } = 0;
    public int TemporaryDefense { get; set; } = 0;
    public int TotalDefense => BaseDefense + BonusDefense + TemporaryDefense;

    public int CriticalChance { get; set; } = 5;
    public float CriticalDamage { get; set; } = 1.5f;

    public int Shield { get; set; } = 0;
    public int Regeneration { get; set; } = 0;
}
