using MothsOath.Core.Common;
using MothsOath.Core.Entities;
using MothsOath.Core.Services;
using System.Text.Json;

namespace MothsOath.Core.Factories;

public class CardFactory
{
    private readonly Dictionary<string, JsonElement> _cardBlueprints;
    private readonly AbilityFactory _abilityFactory;

    public CardFactory(BlueprintLoader blueprintLoader, AbilityFactory abilityFactory)
    {
        _cardBlueprints = blueprintLoader.LoadAllRawBlueprints("Cards");
        _abilityFactory = abilityFactory;
    }

    public BaseCard CreateCard(string blueprintId)
    {
        var blueprintJson = _cardBlueprints[blueprintId];

        string? cardType = blueprintJson.GetProperty("Type").GetString();
        if (string.IsNullOrEmpty(cardType))
        {
            throw new Exception("O campo 'Type' da carta está ausente ou nulo.");
        }

        switch (cardType)
        {
            case "Standard":
                return CreateStandardCard(blueprintJson);
            default:
                throw new Exception($"Tipo de carta desconhecido: {cardType}");
        }
    }

    private StandardCard CreateStandardCard(JsonElement blueprintJson)
    {
        var card = new StandardCard
        {
            Name = blueprintJson.GetProperty("Name").GetString() ?? string.Empty,
            Description = blueprintJson.GetProperty("Description").GetString() ?? string.Empty,
        };

        JsonElement valueElement;

        if (blueprintJson.TryGetProperty("HealthCost", out valueElement) && valueElement.ValueKind != JsonValueKind.Null)
        {
            card.HealthCost = valueElement.GetInt32();
        }

        if (blueprintJson.TryGetProperty("ManaCost", out valueElement) && valueElement.ValueKind != JsonValueKind.Null)
        {
            card.ManaCost = valueElement.GetInt32();
        }

        if (blueprintJson.TryGetProperty("StaminaCost", out valueElement) && valueElement.ValueKind != JsonValueKind.Null)
        {
            card.StaminaCost = valueElement.GetInt32();
        }

        if (blueprintJson.TryGetProperty("GoldCost", out valueElement) && valueElement.ValueKind != JsonValueKind.Null)
        {
            card.GoldCost = valueElement.GetInt32();
        }

        if(blueprintJson.TryGetProperty("Effect", out valueElement) && valueElement.ValueKind != JsonValueKind.Null)
        {
            var abilityBlueprintId = valueElement.GetString();
            if (!string.IsNullOrEmpty(abilityBlueprintId))
            {
                card.Effect = _abilityFactory.GetAbility(abilityBlueprintId);
            }

        }

        return card;
    }
    
}

