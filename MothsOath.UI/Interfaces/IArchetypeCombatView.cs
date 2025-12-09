using MothsOath.Core.Common;

namespace MothsOath.UI.Interfaces;

public interface IArchetypeCombatView
{
    bool IsInSpecialMode();

    bool HandleTargetSelection(BaseCharacter target);
}
