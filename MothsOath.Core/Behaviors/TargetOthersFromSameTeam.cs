using MothsOath.Core.Common;
using MothsOath.Core.States;

namespace MothsOath.Core.Behaviors;

public class TargetOthersFromSameTeam : IBehavior
{
    public string Id => "target_others_from_same_team_behavior";

    public List<BaseCharacter> GetTargets(BaseCharacter source, CombatState gameState)
    {
        var targets = gameState.GetAllCharacters().Where(c => c.Allegiance == source.Allegiance && c.Stats.IsAlive).ToList();
        
        if(targets.Contains(source))
            targets.Remove(source);

        return targets;
    }
}
