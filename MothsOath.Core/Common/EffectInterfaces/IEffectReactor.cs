namespace MothsOath.Core.Common.EffectInterfaces;

/// <summary>
/// Base interface for all effect reactors in the game.
/// All reactive interfaces should inherit from this to enable unified querying and caching.
/// </summary>
public interface IEffectReactor
{
    /// <summary>
    /// Execution priority for this reactor. Higher values execute first.
    /// Typical values:
    /// - 0: Normal priority (default)
    /// - 5: High priority
    /// - 10: Critical priority (executes before everything else)
    /// </summary>
    int Priority { get; set; }
}
