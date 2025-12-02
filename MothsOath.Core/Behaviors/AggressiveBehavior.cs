using MothsOath.Core.Common;
using MothsOath.Core.States;

namespace MothsOath.Core.Behaviors;

public class AggressiveBehavior : IBehavior
{
    public string Id => "aggressive_behavior";

    public List<Character> GetTargets(Character source, CombatState context)
    {
        List<Character> targets = new List<Character>();
        targets.Add(context.Player);
        return targets;
    }
}
