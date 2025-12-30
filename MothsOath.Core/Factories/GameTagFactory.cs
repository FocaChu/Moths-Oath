using MothsOath.Core.Common;
using MothsOath.Core.Services;
using System.Text.Json;

namespace MothsOath.Core.Factories;

public class GameTagFactory
{
    private readonly Dictionary<string, GameTag> _tagBlueprints = new();

    public GameTagFactory(BlueprintCache blueprintCache)
    {
        var tagData = blueprintCache.GetTags();
        LoadTags(tagData);
    }

    private void LoadTags(Dictionary<string, JsonElement> tagData)
    {
        if (tagData.TryGetValue("tags", out var tagsElement))
        {
            if (tagsElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var tagElement in tagsElement.EnumerateArray())
                {
                    ProcessTag(tagElement);
                }
            }
        }
    }

    private void ProcessTag(JsonElement tagElement)
    {
        var id = tagElement.GetProperty("Id").GetString();
        var name = tagElement.GetProperty("Name").GetString();
        var description = tagElement.GetProperty("Description").GetString();
        var hexColor = tagElement.TryGetProperty("HexColor", out var colorElement) && colorElement.ValueKind != JsonValueKind.Null
            ? colorElement.GetString()
            : "#FFFFFF";

        if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(description))
        {
            var tag = new GameTag(id, name, description, hexColor ?? "#FFFFFF");
            _tagBlueprints[id] = tag;
        }
    }

    public GameTag GetTag(string tagId)
    {
        if (!_tagBlueprints.TryGetValue(tagId, out var tag))
        {
            throw new KeyNotFoundException($"Tag com ID '{tagId}' não encontrada.");
        }

        return new GameTag(tag.ID, tag.Name, tag.Description, tag.HexColor);
    }

    public List<GameTag> GetTags(List<string> tagIds)
    {
        var tags = new List<GameTag>();
        foreach (var tagId in tagIds)
        {
            try
            {
                tags.Add(GetTag(tagId));
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"[AVISO] {ex.Message}");
            }
        }
        return tags;
    }

    public bool TagExists(string tagId)
    {
        return _tagBlueprints.ContainsKey(tagId);
    }

    public IReadOnlyDictionary<string, GameTag> GetAllTags()
    {
        return _tagBlueprints;
    }
}
