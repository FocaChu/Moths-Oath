using MothsOath.Core.Common;

namespace MothsOath.Core.Abilities;

public interface IAction
{
    string Id { get; }

    void Execute(ActionContext context);
}
