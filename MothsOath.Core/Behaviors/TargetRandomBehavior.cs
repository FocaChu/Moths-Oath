using MothsOath.Core.Common;
using MothsOath.Core.States;

namespace MothsOath.Core.Behaviors;

public class TargetRandomBehavior : IBehavior   
{
    public string Id => "target_random_behavior";
    public List<Character> GetTargets(Character source, CombatState context)
    {
        List<Character> possibleTargets = context.GetAllCharacters()
            .Where(c => c.IsAlive && c != source)
            .ToList();

        if (possibleTargets.Count == 0)
        {
            return new List<Character>();
        }

        Random rand = new Random();
        int index = rand.Next(possibleTargets.Count);

        return new List<Character> { possibleTargets[index] };
    }
}
