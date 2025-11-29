namespace MothsOath.Core.Common;

public abstract class Character
{
    public Guid Id { get; protected set; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;

    public int CurrentHP { get; set; }

    public int MaxHP { get; set; }

    public int BaseStrength { get; set; }

    public int BonusStrength { get; set; } = 0;

    public int BaseDefense { get; set; }

    public int BonusDefense { get; set; } = 0;
}