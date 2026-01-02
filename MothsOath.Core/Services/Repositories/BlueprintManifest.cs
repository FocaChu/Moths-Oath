namespace MothsOath.Core.Services.Repositories;

/// <summary>
/// Represents a manifest.json file that lists blueprint files in a folder.
/// </summary>
internal class BlueprintManifest
{
    public List<string> Files { get; set; } = new();
}
