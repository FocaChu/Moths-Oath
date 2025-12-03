using MothsOath.Core.Common;

namespace MothsOath.Core.StatusEffect;

public abstract class BaseStatusEffect
{
    public abstract string Id { get; set; }

    public abstract string Name { get; set; }

    public abstract string Description { get; set; }

    public int Level { get; set; }

    public int Duration { get; set; }

    public virtual void TickTime(Character holder)
    {
        if (Duration > 0)
        {
            Duration--;
        }
    }

    public virtual void StackEffect(BaseStatusEffect newEffect)
    {
        Level += newEffect.Level;
        Duration = Math.Max(Duration, newEffect.Duration);
    }
}
