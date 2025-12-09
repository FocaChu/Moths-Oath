using MothsOath.Core.Common;
using MothsOath.Core.States;

namespace MothsOath.Core.Behaviors;

public class TargetOnlyPlayerBehavior : IBehavior
{
    public string Id => "target_only_player_behavior";

    public List<BaseCharacter> GetTargets(BaseCharacter source, CombatState context)
    {
        List<BaseCharacter> targets = new List<BaseCharacter>();
        targets.Add(context.Player);
        return targets;
    }
}
