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

    public int BaseCriticalChance { get; set; } = 5;
    public int BonusCriticalChance { get; set; } = 0;      
    public int TemporaryCriticalChance { get; set; } = 0;
    public int TotalCriticalChance => BaseCriticalChance + BonusCriticalChance + TemporaryCriticalChance;

    public float BaseCriticalDamageMultiplier { get; set; } = 1.5f;
    public float BonusCriticalDamageMultiplier { get; set; } = 0f;
    public float TemporaryCriticalDamageMultiplier { get; set; } = 0f;
    public float TotalCriticalDamageMultiplier => BaseCriticalDamageMultiplier + BonusCriticalDamageMultiplier + TemporaryCriticalDamageMultiplier;

    public int Shield { get; set; } = 0;
    public int Regeneration { get; set; } = 0;
}
