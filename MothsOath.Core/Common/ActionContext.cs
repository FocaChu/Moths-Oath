using MothsOath.Core.States;

namespace MothsOath.Core.Common;

public class ActionContext
{
    public Character Source { get; set; }

    public List<Character> Targets { get; set; }

    public CombatState GameState { get; set; }

    public BaseCard? Card { get; set; }

    public ActionContext(Character source, List<Character> targets, CombatState gameState, BaseCard? card)
    {
        Source = source;
        Targets = targets;
        GameState = gameState;
        Card = card;
    }
}
