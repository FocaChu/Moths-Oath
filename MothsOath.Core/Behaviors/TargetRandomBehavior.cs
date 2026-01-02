using MothsOath.Core.Common;
using MothsOath.Core.States;

namespace MothsOath.Core.Behaviors;

public class TargetRandomBehavior : IBehavior   
{
    public string Id => "target_random_behavior";
    
    public List<BaseCharacter> GetTargets(BaseCharacter source, CombatState context)
    {
        List<BaseCharacter> possibleTargets = context.GetAllCharacters()
            .Where(c => c.Stats.IsAlive && c != source)
            .ToList();

        if (possibleTargets.Count == 0)
        {
            return new List<BaseCharacter>();
        }

        return new List<BaseCharacter> { GameRandom.GetRandomElement(possibleTargets) };
    }
}
