using MothsOath.Core.Models.Blueprints.Common;

namespace MothsOath.Core.Models.Blueprints;

public class DiseaseBlueprint : IBlueprint
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string BehaviorId { get; set; } = string.Empty;

    public List<string> InitialSymptoms { get; set; } = new List<string>();

    public List<string> ViableMutations { get; set; } = new List<string>();
}