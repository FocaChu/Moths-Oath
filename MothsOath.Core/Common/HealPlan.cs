namespace MothsOath.Core.Common;

public class HealPlan
{
    public Character Source { get; set; } = null!;

    public int BaseHealAmount { get; set; } = 0;

    public int FinalHealAmount { get; set; } = 0;

    public bool CanProceed { get; set; } = true;

    public HealPlan(Character source, int baseHealAmount)
    {
        Source = source;
        BaseHealAmount = baseHealAmount;
        FinalHealAmount = baseHealAmount;
    }
}
