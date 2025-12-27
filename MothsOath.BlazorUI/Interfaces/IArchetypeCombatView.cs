using MothsOath.Core.Common;

namespace MothsOath.BlazorUI.Interfaces;

public interface IArchetypeCombatView
{
    bool IsInSpecialMode();

    bool HandleTargetSelection(BaseCharacter target);
}

