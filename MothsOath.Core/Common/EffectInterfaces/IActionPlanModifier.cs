namespace MothsOath.Core.Common.EffectInterfaces;

public interface IActionPlanModifier
{
    int Priority { get; set; }
    void ModifyActionPlan(ActionContext context);
}
