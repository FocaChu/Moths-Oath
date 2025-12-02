namespace MothsOath.Core.Common;

public class ActionPlan
{
    public Character? Source { get; set; }

    public List<Character> BaseTargets { get; set; } = new();

    public List<Character> FinalTargets { get; set; } = new();

    public bool CanProceed { get; set; } = true;

    public bool CanUseSpecial { get; set; } = true;
}
