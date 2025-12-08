using MothsOath.Core.Common;
using MothsOath.Core.Models.Enums;

namespace MothsOath.Core.StatusEffect;

public abstract class BaseStatusEffect
{
    public abstract string Id { get; set; }

    public abstract string Name { get; set; }

    public abstract string Description { get; set; }

    public int Level { get; set; }

    public int Duration { get; set; }

    public abstract bool IsEndless { get; set; }

    public abstract bool IsEchoable { get; set; }

    public abstract bool IsVisible { get; set; }

    public abstract StatusEffectType EffectType { get; set; }

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

    public virtual BaseStatusEffect Clone()
    {
        return (BaseStatusEffect)this.MemberwiseClone();
    }

    public virtual BaseStatusEffect EchoEffect()
    {
        if(!IsEchoable) 
            return null;

        var eco = (BaseStatusEffect)this.MemberwiseClone();

        eco.Level = (int)(eco.Level * 0.5) > 0 ? (int)(eco.Level * 0.5) : 1;
        eco.Duration = (int)(eco.Duration * 0.5) > 0 ? (int)(eco.Duration * 0.5) : 1;

        return eco;
    }

    public virtual void StackEffect(Character owner, BaseStatusEffect newEffect)
    {
        if (!this.IsEchoable && newEffect.IsEchoable)
            this.IsEchoable = true;

        Level += newEffect.Level;
        Duration = Math.Max(Duration, newEffect.Duration);
    }
}
