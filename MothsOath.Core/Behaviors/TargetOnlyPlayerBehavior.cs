using MothsOath.Core.Common;
using MothsOath.Core.States;

namespace MothsOath.Core.Behaviors;

public class TargetOnlyPlayerBehavior : IBehavior
{
    public string Id => "target_only_player_behavior";

    public List<Character> GetTargets(Character source, CombatState context)
    {
        List<Character> targets = new List<Character>();
        targets.Add(context.Player);
        return targets;
    }
}
