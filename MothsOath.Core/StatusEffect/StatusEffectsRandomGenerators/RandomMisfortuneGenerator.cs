using MothsOath.Core.StatusEffect.ConcreteEffects;

namespace MothsOath.Core.StatusEffect.StatusEffectsRandomGenerators;

public static class RandomMisfortuneGenerator
{
    public static BaseStatusEffect GenerateRandomMisfortune(int level, int duration)
    {
        var rng = new Random();
        var misfortunes = new List<BaseStatusEffect>
        {
            new BleedingEffect(level, duration),
            new PoisonEffect(level, duration),
        };

        int index = rng.Next(misfortunes.Count);
        return misfortunes[index];
    }
}