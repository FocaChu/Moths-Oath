using MothsOath.Core.Common;
using MothsOath.Core.Entities;
using MothsOath.Core.Models.Blueprints;
using MothsOath.Core.Models.Enums;
using MothsOath.Core.Services;

namespace MothsOath.Core.Factories;

public class PlayerFactory
{
    private readonly Dictionary<string, RaceBlueprint> _raceBlueprints;
    private readonly Dictionary<string, ArchetypeBlueprint> _archetypeBlueprints;
    private readonly PassiveEffectFactory _passiveEffectFactory;
    private readonly CardFactory _cardFactory;

    public PlayerFactory(CardFactory cardFactory, PassiveEffectFactory passiveEffectFactory, BlueprintLoader blueprintLoader)
    {
        _cardFactory = cardFactory;
        _passiveEffectFactory = passiveEffectFactory;
        _raceBlueprints = blueprintLoader.LoadAllBlueprintsFromFiles<RaceBlueprint>("Races");
        _archetypeBlueprints = blueprintLoader.LoadAllBlueprintsFromFiles<ArchetypeBlueprint>("Archetypes");
    }

    public Player CreatePlayer(string playerName, string raceId, string archetypeId)
    {
        var raceBlueprint = _raceBlueprints[raceId];
        var archetypeBlueprint = _archetypeBlueprints[archetypeId];

        var stats = new Stats
        {
            MaxHealth = raceBlueprint.BaseHealth + archetypeBlueprint.BonusHealth + 25,
            CurrentHealth = raceBlueprint.BaseHealth + archetypeBlueprint.BonusHealth + 25,
            BaseStrength = raceBlueprint.BaseStrength + archetypeBlueprint.BonusStrength + 1,
            BaseKnowledge = raceBlueprint.BaseKnowledge + archetypeBlueprint.BonusKnowledge + 1,
            BaseDefense = raceBlueprint.BaseDefense + archetypeBlueprint.BonusDefense + 1,
            Regeneration = raceBlueprint.BaseRegeneration + archetypeBlueprint.BonusRegeneration,
            BaseCriticalChance = raceBlueprint.BaseCriticalChance + archetypeBlueprint.BonusCriticalChance,
            BaseCriticalDamageMultiplier = raceBlueprint.BaseCriticalDamageMultiplier + archetypeBlueprint.BonusCriticalDamageMultiplier,
        };

        var player = new Player
        {
            Archetype = archetypeBlueprint.Name,
            Race = GetRaceTypeFromId(raceId),
            Level = 1,
            Allegiance = Allegiance.Ally,
            Name = playerName,
            Stats = stats,
            MaxMana = raceBlueprint.BaseMana + archetypeBlueprint.BonusMana,
            CurrentMana = raceBlueprint.BaseMana + archetypeBlueprint.BonusMana,
            MaxStamina = raceBlueprint.BaseStamina + archetypeBlueprint.BonusStamina,
            CurrentStamina = raceBlueprint.BaseStamina + archetypeBlueprint.BonusStamina,
            Gold = 100 + archetypeBlueprint.InitialGold,
            XpMultiplier = 1 + raceBlueprint.BonusXpMultiplier,
        };

        if (raceBlueprint.PassiveEffectId != null)
        {
            var racePassive = _passiveEffectFactory.GetPassiveEffect(raceBlueprint.PassiveEffectId);

            if (racePassive != null && racePassive is not NullPassiveEffect)
                player.PassiveEffects.Add(racePassive);
        }

        if (archetypeBlueprint.PassiveEffectIds != null)
        {
            foreach (var passiveEffectId in archetypeBlueprint.PassiveEffectIds)
            {
                var passiveEffect = _passiveEffectFactory.GetPassiveEffect(passiveEffectId);
                if (passiveEffect != null && passiveEffect is not NullPassiveEffect)
                    player.PassiveEffects.Add(passiveEffect);
            }
        }

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

    private RaceType GetRaceTypeFromId(string raceId)
    {
        return raceId.ToLower() switch
        {
            "ghoul" => RaceType.Ghoul,
            "human" => RaceType.Human,
            "yulkin" => RaceType.Yulkin,
            _ => RaceType.Human
        };

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
