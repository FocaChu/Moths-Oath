namespace MothsOath.Core.PassiveEffects;

public abstract class BasePassiveEffect
{
    public abstract string Id { get; set; }

    public abstract string Name { get; set; }

    public abstract string Description { get; set; }

    public bool IsActive { get; set; } = true;

    public virtual BasePassiveEffect Clone()
    {
        return (BasePassiveEffect)this.MemberwiseClone();
    }
}
