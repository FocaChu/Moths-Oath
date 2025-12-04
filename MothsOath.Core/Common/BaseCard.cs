using MothsOath.Core.Abilities;
using MothsOath.Core.States;

namespace MothsOath.Core.Common;

public class BaseCard
{
    public Guid Id { get;} = Guid.NewGuid();

    public int HealthCost { get; set; } = 0;

    public int ManaCost { get; set; } = 0;

    public int StaminaCost { get; set; } = 0;

    public int GoldCost { get; set; } = 0;

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public BaseAction Action { get; set; } = null!;

    public virtual void PlayEffect(ActionContext context)
    {
        Action.Execute(context);
    }
}
