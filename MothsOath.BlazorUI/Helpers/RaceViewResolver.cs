using MothsOath.Core.Models.Enums;
using MothsOath.BlazorUI.Components.RacesCombatUI;

namespace MothsOath.BlazorUI.Helpers;

public static class RaceViewResolver
{
    private static readonly Dictionary<RaceType, Type> _raceMap = new()
    {
        { RaceType.Ghoul, typeof(GhoulRaceUI) },
        { RaceType.Yulkin, typeof(YulkinRaceUI) },
        { RaceType.Human, typeof(EmptyRaceUI) }
    };

    public static Type GetViewType(RaceType race)
    {
        return _raceMap.TryGetValue(race, out var viewType) ? viewType : null;
    }
}
