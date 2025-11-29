namespace MothsOath.Core.Common;

public abstract class Character
{
    public Guid Id { get; protected set; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;

    public int CurrentHP { get; set; }

    public int MaxHP { get; set; }
}
