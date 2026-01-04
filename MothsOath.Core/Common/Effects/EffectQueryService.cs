using MothsOath.Core.Common.EffectInterfaces;
using MothsOath.Core.PassiveEffects;

namespace MothsOath.Core.Common.Effects;

/// <summary>
/// Stateless service for querying and filtering effects.
/// Optimized for performance with minimal allocations using for loops instead of LINQ.
/// </summary>
public static class EffectQueryService
{
    /// <summary>
    /// Queries both StatusEffects and PassiveEffects for a specific reactor interface,
    /// then sorts by Priority (descending - higher priority executes first).
    /// Uses manual iteration instead of LINQ for better performance.
    /// </summary>
    public static IReadOnlyList<TReactor> QueryAndSort<TReactor>(
        List<StatusEffect.BaseStatusEffect> statusEffects,
        List<BasePassiveEffect> passiveEffects)
        where TReactor : class, IEffectReactor
    {
        // Pre-size capacity to avoid reallocations
        var capacity = statusEffects.Count + passiveEffects.Count;
        var results = new List<TReactor>(capacity);

        // Query StatusEffects using for loop (faster than LINQ)
        for (int i = 0; i < statusEffects.Count; i++)
        {
            if (statusEffects[i] is TReactor reactor)
            {
                results.Add(reactor);
            }
        }

        // Query PassiveEffects
        for (int i = 0; i < passiveEffects.Count; i++)
        {
            if (passiveEffects[i] is TReactor reactor)
            {
                results.Add(reactor);
            }
        }

        // Sort by priority (descending) - only if needed
        if (results.Count > 1)
        {
            results.Sort((a, b) => b.Priority.CompareTo(a.Priority));
        }

        return results;
    }

    /// <summary>
    /// Filters out inactive StatusEffects (those with Duration &lt;= 0 or Level &lt;= 0).
    /// PassiveEffects are always active.
    /// Uses yield return for lazy evaluation.
    /// </summary>
    public static IEnumerable<TReactor> FilterActive<TReactor>(
        IReadOnlyList<TReactor> reactors)
        where TReactor : class, IEffectReactor
    {
        for (int i = 0; i < reactors.Count; i++)
        {
            var reactor = reactors[i];
            
            // Check if it's a BaseStatusEffect and if it's active
            if (reactor is StatusEffect.BaseStatusEffect statusEffect)
            {
                if (!statusEffect.IsActive())
                    continue;
            }

            yield return reactor;
        }
    }
}
