using MothsOath.Core.Common;
using MothsOath.Core.States;

namespace MothsOath.Core.Behaviors;

public class TargetBleedingOrHurtCharacter : IBehavior
{
    public string Id => "target_bleeding_or_hurt_character_behavior";

    public List<Character> GetTargets(Character source, CombatState context)
    {
        var targets = new List<Character>();

        foreach (var character in context.GetAllCharacters())
        {
            var isHurt = character.Stats.CurrentHealth < character.Stats.MaxHealth;
            var hasBleeding = character.StatusEffects.Any(s => s.Name == "Sangramento");

            if (character == source)
                continue;

            if (isHurt || hasBleeding)
            {
                targets.Add(character);
            }
        }

        Console.WriteLine($"[DEBUG] Alvos encontrados: {string.Join(", ", targets.Select(t => t.Name))}");
        return targets;
    }
}
