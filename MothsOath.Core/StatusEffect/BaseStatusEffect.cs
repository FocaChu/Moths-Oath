namespace MothsOath.Core.StatusEffect;

public abstract class BaseStatusEffect
{
    public Guid Id { get; set; } 

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int Level { get; set; }

    public int Duration { get; set; }

    public virtual void TickTime()
    {
        if (Duration > 0)
        {
            Duration--;
        }
    }
}
