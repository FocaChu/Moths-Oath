using MothsOath.Core.Common;
using MothsOath.Core.Entities;
using MothsOath.Core.Models.Blueprints;
using MothsOath.Core.Services;

namespace MothsOath.Core.Factories;

public class PlayerFactory
{
    private readonly Dictionary<string, RaceBlueprint> _raceBlueprints;
    private readonly Dictionary<string, ArchetypeBlueprint> _archetypeBlueprints;
    private readonly CardFactory _cardFactory;

    public PlayerFactory(CardFactory cardFactory, BlueprintLoader blueprintLoader)
    {
        _cardFactory = cardFactory;
        _raceBlueprints = blueprintLoader.LoadAllBlueprintsFromFiles<RaceBlueprint>("Races");
        _archetypeBlueprints = blueprintLoader.LoadAllBlueprintsFromFiles<ArchetypeBlueprint>("Archetypes");
    }

    public Player CreatePlayer(string playerName, string raceId, string archetypeId)
    {
        var raceBlueprint = _raceBlueprints[raceId];
        var archetypeBlueprint = _archetypeBlueprints[archetypeId];

        var stats = new Stats
        {
            MaxHealth = raceBlueprint.BaseHealth + archetypeBlueprint.BonusHealth,
            CurrentHealth = raceBlueprint.BaseHealth + archetypeBlueprint.BonusHealth,
            BaseStrength = raceBlueprint.BaseStrength + archetypeBlueprint.BonusStrength,
            BaseKnowledge = raceBlueprint.BaseKnowledge + archetypeBlueprint.BonusKnowledge,
            BaseResistance = raceBlueprint.BaseResistance + archetypeBlueprint.BonusResistance,
            Regeneration = raceBlueprint.BaseRegeneration + archetypeBlueprint.BonusRegeneration,
        };

        var player = new Player
        {
            Name = playerName,
            Stats = stats,
            MaxMana = raceBlueprint.BaseMana + archetypeBlueprint.BonusMana,
            CurrentMana = raceBlueprint.BaseMana + archetypeBlueprint.BonusMana,
            MaxStamina = raceBlueprint.BaseStamina + archetypeBlueprint.BonusStamina,
            CurrentStamina = raceBlueprint.BaseStamina + archetypeBlueprint.BonusStamina,
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

    public List<RaceBlueprint> GetAllRaceBlueprints()
    {
        return _raceBlueprints.Values.ToList();
    }

    public List<ArchetypeBlueprint> GetAllArchetypeBlueprints()
    {
        return _archetypeBlueprints.Values.ToList();
    }
}
