using MothsOath.Core.Entities;
using MothsOath.Core.Models.Blueprints;
using MothsOath.Core.Services;

namespace MothsOath.Core.Factories;

public class PlayerFactory
{
    private readonly Dictionary<string, RaceBlueprint> _raceBlueprints;
    private readonly Dictionary<string, ArchetypeBlueprint> _classBlueprints;
    private readonly CardFactory _cardFactory;

    public PlayerFactory(CardFactory cardFactory, BlueprintLoader blueprintLoader)
    {
        _cardFactory = cardFactory;
        _raceBlueprints = blueprintLoader.LoadAllBlueprintsFromFiles<RaceBlueprint>("Races");
        _classBlueprints = blueprintLoader.LoadAllBlueprintsFromFiles<ArchetypeBlueprint>("Archetypes");
    }

    public Player CreatePlayer(string playerName, string raceId, string classId)
    {
        var raceBlueprint = _raceBlueprints[raceId];
        var archetypeBlueprint = _classBlueprints[classId];

        var player = new Player
        {
            Name = playerName,
            MaxHealth = raceBlueprint.BaseHealth + archetypeBlueprint.BonusHealth,
            CurrentHealth = raceBlueprint.BaseHealth + archetypeBlueprint.BonusHealth,
            MaxMana = raceBlueprint.BaseMana + archetypeBlueprint.BonusMana,
            CurrentMana = raceBlueprint.BaseMana + archetypeBlueprint.BonusMana,
            MaxStamina = raceBlueprint.BaseStamina + archetypeBlueprint.BonusStamina,
            CurrentStamina = raceBlueprint.BaseStamina + archetypeBlueprint.BonusStamina,
            BaseStrength = raceBlueprint.BaseStrength + archetypeBlueprint.BonusStrength,
            BaseResistance = raceBlueprint.BaseResistance + archetypeBlueprint.BonusResistance,
            Gold = 100 + archetypeBlueprint.InitialGold,
            XpMultiplier = 1 + raceBlueprint.BonusXpMultiplier,
        };

        List<string> allCardsId = new List<string>();
        allCardsId.AddRange(raceBlueprint.StartingCardIds);
        allCardsId.AddRange(archetypeBlueprint.StartingCardIds);

        foreach (var cardId in allCardsId)
        {
            var card = _cardFactory.CreateCard(cardId);
            player.Deck.Add(card);
        }

        return player;
    }
}
