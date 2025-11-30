using MothsOath.Core.Abilities;
using MothsOath.Core.Common;

namespace MothsOath.Core.Entities;

public class Enemy : Character
{
    public IAbility BasicAttack { get; set; } = null!;

    public IAbility SpecialAbility { get; set; } = null!;

    public int SpecialAbilityCooldown { get; set; }

    public int CurrentCooldown { get; set; }


    public void TakeTurn(GameStateManager gameState)
    {
        var target = gameState.Player;

        #nullable disable
        if (CurrentCooldown <= 0)
        {
            SpecialAbility.Execute(this, target, gameState);
            CurrentCooldown = SpecialAbilityCooldown;
        }
        else
        {
            BasicAttack.Execute(this, target, gameState);
        }
        #nullable disable

        CurrentCooldown--;
    }
}
