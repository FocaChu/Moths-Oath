using MothsOath.Core.Common;
using MothsOath.Core.StatusEffect.ConcreteEffects;

namespace MothsOath.Core.StatusEffect.StatusEffectsRandomGenerators;

public static class RandomMisfortuneGenerator
{
    public static BaseStatusEffect GenerateRandomMisfortune(int level, int duration)
    {
        var misfortunes = new List<BaseStatusEffect>
        {
            new BleedingEffect(level, duration),
            new PoisonEffect(level, duration),
        };

        return GameRandom.GetRandomElement(misfortunes);
    }
}