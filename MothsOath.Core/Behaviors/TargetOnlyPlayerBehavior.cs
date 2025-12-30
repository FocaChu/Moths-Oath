using MothsOath.Core.Common;
using MothsOath.Core.States;

namespace MothsOath.Core.Behaviors;

public class TargetOnlyPlayerBehavior : IBehavior
{
    public string Id => "target_only_player_behavior";

    public List<BaseCharacter> GetTargets(BaseCharacter source, CombatState context)
    {
        List<BaseCharacter> targets = context.GetAllCharacters()
            .Where(c => c.Stats.IsAlive && c.Tags.Any(t => t.ID == "player"))
            .ToList();

        return targets;
    }
}
