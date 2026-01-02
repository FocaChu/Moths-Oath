using MothsOath.Core.Common;
using MothsOath.Core.StatusEffect.ConcreteStatusEffects;

namespace MothsOath.Core.StatusEffect.StatusEffectsRandomGenerators;

public static class RandomGloryGenerator
{
    public static BaseStatusEffect GenerateRandomGlory(int level, int duration)
    {
        var glories = new List<BaseStatusEffect>
        {
            new HealingAuraEffect(level, duration),
            new SuperForceEffect(level, duration)
        };

        return GameRandom.GetRandomElement(glories);
    }
}