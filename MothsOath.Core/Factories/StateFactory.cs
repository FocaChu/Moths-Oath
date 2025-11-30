using MothsOath.Core.Entities;
using MothsOath.Core.States;

namespace MothsOath.Core.Factories;

public class StateFactory
{
    private readonly AbilityFactory _abilityFactory;
    private readonly EnemyFactory _enemyFactory;

    public StateFactory(
        AbilityFactory abilityFactory,
        EnemyFactory enemyFactory)
    {
        _abilityFactory = abilityFactory;
        _enemyFactory = enemyFactory;
    }

    public MainMenuState CreateMainMenuState(GameManager gameManager)
    {
        return new MainMenuState(gameManager, this); 
    }

    public CombatState CreateCombatState(GameManager gameManager, Player player)
    {
        return new CombatState(gameManager, _enemyFactory, this, player);
    }
}