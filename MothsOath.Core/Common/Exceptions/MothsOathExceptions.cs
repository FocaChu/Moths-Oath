namespace MothsOath.Core.Common.Exceptions;

/// <summary>
/// Base exception for all MothsOath Core exceptions.
/// </summary>
public abstract class MothsOathException : Exception
{
    protected MothsOathException() { }
    
    protected MothsOathException(string message) : base(message) { }
    
    protected MothsOathException(string message, Exception innerException) 
        : base(message, innerException) { }
}

/// <summary>
/// Exception thrown when a blueprint is not found.
/// </summary>
public class BlueprintNotFoundException : MothsOathException
{
    public string BlueprintId { get; }
    public string BlueprintType { get; }

    public BlueprintNotFoundException(string blueprintId, string blueprintType)
        : base($"Blueprint '{blueprintId}' of type '{blueprintType}' was not found.")
    {
        BlueprintId = blueprintId;
        BlueprintType = blueprintType;
    }
}

/// <summary>
/// Exception thrown when a blueprint is invalid or corrupted.
/// </summary>
public class InvalidBlueprintException : MothsOathException
{
    public string BlueprintId { get; }
    public string Reason { get; }

    public InvalidBlueprintException(string blueprintId, string reason)
        : base($"Blueprint '{blueprintId}' is invalid: {reason}")
    {
        BlueprintId = blueprintId;
        Reason = reason;
    }

    public InvalidBlueprintException(string blueprintId, string reason, Exception innerException)
        : base($"Blueprint '{blueprintId}' is invalid: {reason}", innerException)
    {
        BlueprintId = blueprintId;
        Reason = reason;
    }
}

/// <summary>
/// Exception thrown when an action/ability is not found.
/// </summary>
public class ActionNotFoundException : MothsOathException
{
    public string ActionId { get; }

    public ActionNotFoundException(string actionId)
        : base($"Action with ID '{actionId}' was not found.")
    {
        ActionId = actionId;
    }
}

/// <summary>
/// Exception thrown when a behavior is not found.
/// </summary>
public class BehaviorNotFoundException : MothsOathException
{
    public string BehaviorId { get; }

    public BehaviorNotFoundException(string behaviorId)
        : base($"Behavior with ID '{behaviorId}' was not found.")
    {
        BehaviorId = behaviorId;
    }
}

/// <summary>
/// Exception thrown when a passive effect is not found.
/// </summary>
public class PassiveEffectNotFoundException : MothsOathException
{
    public string EffectId { get; }

    public PassiveEffectNotFoundException(string effectId)
        : base($"Passive effect with ID '{effectId}' was not found.")
    {
        EffectId = effectId;
    }
}

/// <summary>
/// Exception thrown when a game tag is not found.
/// </summary>
public class GameTagNotFoundException : MothsOathException
{
    public string TagId { get; }

    public GameTagNotFoundException(string tagId)
        : base($"Game tag with ID '{tagId}' was not found.")
    {
        TagId = tagId;
    }
}

/// <summary>
/// Exception thrown when there's an issue loading blueprints.
/// </summary>
public class BlueprintLoadException : MothsOathException
{
    public string FolderPath { get; }

    public BlueprintLoadException(string folderPath, string message)
        : base($"Failed to load blueprints from '{folderPath}': {message}")
    {
        FolderPath = folderPath;
    }

    public BlueprintLoadException(string folderPath, string message, Exception innerException)
        : base($"Failed to load blueprints from '{folderPath}': {message}", innerException)
    {
        FolderPath = folderPath;
    }
}

/// <summary>
/// Exception thrown when a combat state is invalid.
/// </summary>
public class InvalidCombatStateException : MothsOathException
{
    public InvalidCombatStateException(string message) : base(message) { }
    
    public InvalidCombatStateException(string message, Exception innerException) 
        : base(message, innerException) { }
}

/// <summary>
/// Exception thrown when a card action is invalid.
/// </summary>
public class InvalidCardActionException : MothsOathException
{
    public string CardName { get; }
    public string Reason { get; }

    public InvalidCardActionException(string cardName, string reason)
        : base($"Cannot play card '{cardName}': {reason}")
    {
        CardName = cardName;
        Reason = reason;
    }
}
