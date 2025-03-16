using LethalLevelLoader;

namespace DynamicMoonRatings.Modules.CalculationsV1
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
