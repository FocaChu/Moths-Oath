using MothsOath.Core.Behaviors;
using MothsOath.Core.Common;
using MothsOath.Core.States;

namespace MothsOath.Core.Factories;

public class BehaviorFactory
{
    private readonly Dictionary<string, IBehavior> _behaviors;

    public BehaviorFactory()
    {
        _behaviors = new Dictionary<string, IBehavior>();

        var aggressiveBehavior = new TargetOnlyPlayerBehavior();
        _behaviors.Add(aggressiveBehavior.Id, aggressiveBehavior);

        var randomBehavior = new TargetRandomBehavior();
        _behaviors.Add(randomBehavior.Id, randomBehavior);

        Console.WriteLine($"BehaviorFactory initialized. {_behaviors.Count} behaviors loaded.");
    }

    public IBehavior GetBehavior(string behaviorId)
    {
        if (_behaviors.TryGetValue(behaviorId, out var behavior))
        {
            return behavior;
        }
        Console.WriteLine($"[CRITICAL ERROR] Behavior with ID '{behaviorId}' not found in BehaviorFactory!");
        return new NullBehavior(behaviorId);
    }
}

public class NullBehavior : IBehavior
{
    public string Id { get; }

    public NullBehavior(string missingId)
    {
        Id = $"missing_ability_{missingId}";
    }

    public List<Character> GetTargets(Character source, CombatState context)
    {
        Console.WriteLine($"[AVISO] Tentativa de executar um comportamento não encontrado com ID '{Id}'. Nenhuma ação foi tomada.");
        return null;
    }
}