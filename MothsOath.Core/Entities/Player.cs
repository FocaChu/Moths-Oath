using MothsOath.Core.Common;

namespace MothsOath.Core.Entities;

public class Player : Character
{
    public List<BaseCard> Hand { get; private set; } = new List<BaseCard>();
}
