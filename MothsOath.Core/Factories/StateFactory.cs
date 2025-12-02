using MothsOath.Core.Entities;
using MothsOath.Core.States;

namespace MothsOath.Core.Factories;

public class StateFactory
{
    private readonly AbilityFactory _abilityFactory;
    private readonly EnemyFactory _enemyFactory;
    private readonly PlayerFactory _playerFactory;

    public StateFactory(
        AbilityFactory abilityFactory,
        EnemyFactory enemyFactory,
        PlayerFactory playerFactory)
    {
        _abilityFactory = abilityFactory;
        _enemyFactory = enemyFactory;
        _playerFactory = playerFactory;
    }

    public MainMenuState CreateMainMenuState(GameStateManager gameManager)
    {
        return new MainMenuState(gameManager, _playerFactory, this); 
    }

    public CombatState CreateCombatState(GameStateManager gameManager, Player player)
    {
        return new CombatState(gameManager, _enemyFactory, this, player);
    }
}