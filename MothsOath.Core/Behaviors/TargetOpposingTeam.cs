using MothsOath.Core.Common;
using MothsOath.Core.Entities;
using MothsOath.Core.Models.Enums;
using MothsOath.Core.States;

namespace MothsOath.Core.Behaviors;

public class TargetOpposingTeam : IBehavior
{
    public string Id => "target_opposing_team_behavior";

    public List<BaseCharacter> GetTargets(BaseCharacter source, CombatState gameState)
    {
        if(source.Allegiance == Allegiance.Ally) 
        {
            var availableTargets = gameState.GetAllCharacters()
                .Where(c => c.Allegiance == Allegiance.Enemy)
                .ToList();

            Console.WriteLine("Enemies found: " + string.Join(", ", availableTargets.Select(e => e.Name)));
            return availableTargets;
        }
        else if(source.Allegiance == Allegiance.Enemy) 
        {
            var availableTargets = gameState.GetAllCharacters().Where(c => c.Stats.IsAlive && c.Allegiance == Allegiance.Ally).ToList();

            Console.WriteLine("Allies found: " + string.Join(", ", availableTargets.Select(a => a.Name)));
            return availableTargets;
        }

        return new List<BaseCharacter>();
    }
}
