using MothsOath.Core.Common;
using MothsOath.Core.States;

namespace MothsOath.Core.Behaviors;

public interface IBehavior
{
    string Id { get; }

    List<BaseCharacter> GetTargets(BaseCharacter source, CombatState context);
}
