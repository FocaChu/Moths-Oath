namespace MothsOath.Core.Common;

public class Stats
{
    public int MaxHealth { get; set; }
    public int CurrentHealth { get; set; } = 0;
    public bool IsAlive => CurrentHealth > 0;


    public int BaseStrength { get; set; }
    public int BonusStrength { get; set; } = 0;
    public int TotalStrength => BaseStrength + BonusStrength;


    public int BaseKnowledge { get; set; }
    public int BonusKnowledge { get; set; } = 0;
    public int TotalKnowledge => BaseKnowledge + BonusKnowledge;


    public int BaseResistance { get; set; }
    public int BonusResistance { get; set; } = 0;
    public int TotalResistance => BaseResistance + BonusResistance;


    public int Shield { get; set; } = 0;
    public int Regeneration { get; set; } = 0;
}
