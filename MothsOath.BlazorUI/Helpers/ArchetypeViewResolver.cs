using MothsOath.Core.Entities.Archetypes;
using MothsOath.BlazorUI.Components.ArchetypesCombatUI;

namespace MothsOath.BlazorUI.Helpers;

public static class ArchetypeViewResolver
{
    private static readonly Dictionary<Type, Type> _viewMap = new()
    {
        { typeof(Doctor), typeof(DoctorCombatUI) },
        { typeof(BellRinger), typeof(BellRingerCombatUI) },
        { typeof(Narrator), typeof(NarratorCombatUI) }
    };

    public static Type GetViewType(object player)
    {
        if (player == null) return null;

        var type = player.GetType();

        if (_viewMap.TryGetValue(type, out var viewType))
        {
            return viewType;
        }

        return null;
    }
}
