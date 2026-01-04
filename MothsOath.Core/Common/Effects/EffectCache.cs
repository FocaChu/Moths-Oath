using MothsOath.Core.Common.EffectInterfaces;

namespace MothsOath.Core.Common.Effects;

/// <summary>
/// Caches interface queries for a character's effects to avoid repeated LINQ operations.
/// Thread-safe via lazy initialization pattern with double-check locking.
/// Invalidates cache when effects are added or removed.
/// </summary>
public sealed class EffectCache
{
    private readonly BaseCharacter _owner;
    private readonly Dictionary<Type, object> _cachedReactors = new();
    private readonly object _lock = new();

    public EffectCache(BaseCharacter owner)
    {
        _owner = owner ?? throw new ArgumentNullException(nameof(owner));
    }

    /// <summary>
    /// Gets all effects of a specific reactor interface type, cached and sorted by priority.
    /// First call will query and cache, subsequent calls return cached result (O(1) lookup).
    /// </summary>
    /// <typeparam name="TReactor">The reactor interface type to query for</typeparam>
    /// <returns>Read-only list of reactors sorted by priority (descending)</returns>
    public IReadOnlyList<TReactor> GetReactors<TReactor>() where TReactor : class, IEffectReactor
    {
        var interfaceType = typeof(TReactor);

        // Fast path: return cached if exists (lock-free read)
        if (_cachedReactors.TryGetValue(interfaceType, out var cached))
        {
            return (IReadOnlyList<TReactor>)cached;
        }

        // Slow path: build cache (locked)
        lock (_lock)
        {
            // Double-check after acquiring lock (another thread might have cached it)
            if (_cachedReactors.TryGetValue(interfaceType, out cached))
            {
                return (IReadOnlyList<TReactor>)cached;
            }

            // Query and sort effects
            var reactors = EffectQueryService.QueryAndSort<TReactor>(
                _owner.StatusEffects,
                _owner.PassiveEffects
            );

            _cachedReactors[interfaceType] = reactors;
            return reactors;
        }
    }

    /// <summary>
    /// Invalidates all cached queries. Call when effects are added or removed.
    /// This forces the next GetReactors call to re-query and re-cache.
    /// </summary>
    public void Invalidate()
    {
        lock (_lock)
        {
            _cachedReactors.Clear();
        }
    }

    /// <summary>
    /// Invalidates cache for a specific interface type.
    /// Useful when you know only certain reactors changed.
    /// </summary>
    /// <typeparam name="TReactor">The reactor interface type to invalidate</typeparam>
    public void InvalidateFor<TReactor>() where TReactor : class, IEffectReactor
    {
        lock (_lock)
        {
            _cachedReactors.Remove(typeof(TReactor));
        }
    }

    /// <summary>
    /// Returns the number of cached reactor types. Useful for debugging.
    /// </summary>
    public int CachedCount
    {
        get
        {
            lock (_lock)
            {
                return _cachedReactors.Count;
            }
        }
    }
}
