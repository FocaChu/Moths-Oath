using MothsOath.Core.Entities;
using MothsOath.Core.Services;
using MothsOath.Core.States;

namespace MothsOath.Core.Factories;

public class StateFactory
{
    private readonly ActionFactory _abilityFactory;
    private readonly EnemyFactory _enemyFactory;
    private readonly PlayerFactory _playerFactory;
    private readonly BehaviorFactory _behaviorFactory;
    private readonly BlueprintLoader _blueprintLoader;

    public StateFactory(
        ActionFactory abilityFactory,
        EnemyFactory enemyFactory,
        PlayerFactory playerFactory,
        BehaviorFactory behaviorFactory,
        BlueprintLoader blueprintLoader)
    {
        _abilityFactory = abilityFactory;
        _enemyFactory = enemyFactory;
        _playerFactory = playerFactory;
        _behaviorFactory = behaviorFactory;
        _blueprintLoader = blueprintLoader;
    }

    public MainMenuState CreateMainMenuState(GameStateManager gameManager)
    {
        return new MainMenuState(gameManager, _playerFactory, this); 
    }

    public DifficultySelectionState CreateDifficultySelectionState(GameStateManager gameManager)
    {
        return new DifficultySelectionState(gameManager, this);
    }

    public CombatState CreateCombatState(GameStateManager gameManager, Player player)
    {
        return new CombatState(gameManager, _enemyFactory, this, player);
    }

    public CombatResultState CreateCombatResultState(GameStateManager gameManager, Player player, int totalXp, int totalGold, int turnCount, int enemiesDefeated)
    {
        return new CombatResultState(gameManager, this, player, totalXp, totalGold, turnCount, enemiesDefeated);
    }

    public PlayerCreationState CreatePlayerCreationState(GameStateManager gameManager)
    {
        return new PlayerCreationState(gameManager, _playerFactory, _blueprintLoader);
    }

    public DoctorCreationState CreateDoctorCreationState(GameStateManager gameManager, Player basePlayer)
    {
        return new DoctorCreationState(gameManager, _blueprintLoader, _behaviorFactory, basePlayer);
    }
}