using LethalLevelLoader;

namespace DynamicMoonRatings.Modules.Calculations
{
    internal class Time
    {
        internal static float TimeCalc(ExtendedLevel level)
        {
            float TimeModifier = level.SelectableLevel.DaySpeedMultiplier;
            return TimeModifier;
        }
    }
}
