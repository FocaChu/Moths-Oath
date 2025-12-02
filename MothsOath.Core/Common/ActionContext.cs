using MothsOath.Core.States;

namespace MothsOath.Core.Common;

public class ActionContext
{
    public Character Source { get; set; }

    public Character Target { get; set; }

    public CombatState GameState { get; set; }

    public BaseCard? Card { get; set; }

    public ActionContext(Character source, Character target, CombatState gameState, BaseCard? card)
    {
        Source = source;
        Target = target;
        GameState = gameState;
        Card = card;
    }
}
