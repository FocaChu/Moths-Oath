using MothsOath.Core.StatusEffect.ConcreteEffects;
using MothsOath.Core.StatusEffect.ConcreteStatusEffects;

namespace MothsOath.Core.StatusEffect.StatusEffectsRandomGenerators;

public static class RandomGloryGenerator
{
    public static BaseStatusEffect GenerateRandomGlory(int level, int duration)
    {
        var rng = new Random();
        var glories = new List<BaseStatusEffect>
        {
            new HealingAuraEffect(level, duration),
            new SuperForceEffect(level, duration)
        };

        int index = rng.Next(glories.Count);
        return glories[index];
    }
}