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

    public ActionPlan? Plan { get; set; }

    public bool CanReactTarget { get; set; }

    public bool CanReactSource { get; set; }

    public ActionContext(Character source, List<Character> baseTargets, CombatState gameState, BaseCard? card, ActionPlan? plan, bool canReactTarget, bool canReactSource)
    {
        Source = source;
        BaseTargets = baseTargets;
        FinalTargets = baseTargets;
        GameState = gameState;
        Card = card;
        Plan = plan;
        CanReactTarget = canReactTarget;
        CanReactSource = canReactSource;
    }
}
