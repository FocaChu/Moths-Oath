using MothsOath.Core.Common.Plans;
using MothsOath.Core.States;

namespace MothsOath.Core.Common;

public class ActionContext
{
    public Character Source { get; set; }

    public List<Character> BaseTargets { get; set; }

    public List<Character> FinalTargets { get; set; } = new List<Character>();

    public CombatState GameState { get; set; }

    public BaseCard? Card { get; set; }

    public bool CanUseSpecial { get; set; }

    public bool CanProceed { get; set; } = true;

    public bool CanOutgoingModifiers { get; set; } = true;

    public bool CanIncomingModifiers { get; set; } = true;

    public bool CanRecievedReactors { get; set; } = true;

    public bool CanDealtReactors { get; set; } = true;

    public ActionContext(Character source, List<Character> baseTargets, CombatState gameState, BaseCard? card)
    {
        Source = source;
        BaseTargets = baseTargets;
        FinalTargets = baseTargets;
        GameState = gameState;
        Card = card;
    }
}
