namespace MothsOath.Core.Common.Plans;

public class HealPlan
{
    public int BaseHealAmount { get; set; } = 0;

    public int FinalHealAmount { get; set; } = 0;

    public bool CanProceed { get; set; } = true;

    public HealPlan(int baseHealAmount)
    {
        BaseHealAmount = baseHealAmount;
        FinalHealAmount = baseHealAmount;
    }
}
