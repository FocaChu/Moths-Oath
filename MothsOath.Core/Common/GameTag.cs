namespace MothsOath.Core.Common;

public class GameTag
{
    public string ID { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public string HexColor { get; set; } = "#FFFFFF";

    public GameTag(string id, string name, string description, string hexColor)
    {
        ID = id;
        Name = name;
        Description = description;
        HexColor = hexColor;
    }
}
