namespace MothsOath.Core.Common.Plans;

public class DamagePlan
{
    public int BaseDamageAmount { get; set; } = 0;

    public int FinalDamageAmount { get; set; } = 0;

    public bool CanProceed { get; set; } = true;

    public bool BypassResistance { get; set; } = false;

    public DamagePlan(int baseDamageAmount, bool bypassResistance)
    {
        BaseDamageAmount = baseDamageAmount;
        FinalDamageAmount = baseDamageAmount;
        BypassResistance = bypassResistance;
    }
}
