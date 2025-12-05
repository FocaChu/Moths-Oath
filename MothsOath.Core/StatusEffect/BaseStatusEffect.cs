using MothsOath.Core.Common;

namespace MothsOath.Core.StatusEffect;

public abstract class BaseStatusEffect
{
    public abstract string Id { get; }

    public abstract string Name { get; }

    public abstract string Description { get; }

    public int Level { get; set; }

    public int Duration { get; set; }

    public abstract bool IsEndless { get; set; }

    public virtual bool IsActive()
    {
        if(Duration <= 0 || Level <= 0)
        {
            return false;
        }

        return true;
    }

    public virtual void TickTime(Character holder)
    {
        if (Duration > 0 && !IsEndless)
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
