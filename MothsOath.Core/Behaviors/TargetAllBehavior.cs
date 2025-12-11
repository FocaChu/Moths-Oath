using MothsOath.Core.Common;
using MothsOath.Core.States;

namespace MothsOath.Core.Behaviors;

public class TargetAllBehavior : IBehavior   
{
    public string Id => "target_random_behavior";
    public List<BaseCharacter> GetTargets(BaseCharacter source, CombatState context)
    {
        List<BaseCharacter> allTargets = context.GetAllCharacters()
            .Where(c => c.Stats.IsAlive && c != source)
            .ToList();

        return allTargets;
    }
}
