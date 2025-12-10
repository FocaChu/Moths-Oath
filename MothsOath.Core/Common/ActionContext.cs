using MothsOath.Core.Common.Plans;
using MothsOath.Core.States;

namespace MothsOath.Core.Common;

public class ActionContext
{
    public BaseCharacter Source { get; set; }

    public List<BaseCharacter> BaseTargets { get; set; }

    public List<BaseCharacter> FinalTargets { get; set; } = new List<BaseCharacter>();

    public CombatState GameState { get; set; }

    public BaseCard? Card { get; set; }

    public bool CanUseSpecial { get; set; } = true;

    public bool CanProceed { get; set; } = true;

    public bool CanOutgoingModifiers { get; set; } = true;

    public bool CanIncomingModifiers { get; set; } = true;

    public bool CanRecievedReactors { get; set; } = true;

    public bool CanDealtReactors { get; set; } = true;

    public bool CanDeathReactors { get; set; } = true;

    public ActionContext(BaseCharacter source, List<BaseCharacter> baseTargets, CombatState gameState, BaseCard? card)
    {
        Source = source;
        BaseTargets = baseTargets;
        FinalTargets = baseTargets;
        GameState = gameState;
        Card = card;
    }
}
