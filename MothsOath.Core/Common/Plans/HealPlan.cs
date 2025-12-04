namespace MothsOath.Core.Common.Plans;

public class HealPlan
{
    public int BaseHealAmount { get; set; } = 0;

    public int FinalHealAmount { get; set; } = 0;

    public bool CanProceed { get; set; } = true;

    public HealPlan(int baseHealAmount, bool canProceed)
    {
        BaseHealAmount = baseHealAmount;
        FinalHealAmount = baseHealAmount;
        CanProceed = canProceed;
    }
}
