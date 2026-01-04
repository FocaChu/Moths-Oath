namespace MothsOath.Core.Common.EffectInterfaces;

public interface IActionPlanModifier : IEffectReactor
{
    void ModifyActionPlan(ActionContext context);
}
