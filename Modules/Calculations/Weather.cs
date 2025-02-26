using LethalLevelLoader;

namespace DynamicMoonRatings.Modules.Calculations
{
    internal class Weather
    {
        internal static float WeatherCalc(ExtendedLevel level)
        {
            string weather = level.SelectableLevel.currentWeather.ToString();
            float weatherModifier = 1f;
            if (weather == "Eclipsed")
            {
                weatherModifier = 1.5f;
            }
            else if (weather == "Flooded" || weather == "Stormy")
            {
                weatherModifier = 1.25f;
            }
            else if (weather == "Rainy")
            {
                weatherModifier = 1.1f;
            }
            else if (weather == "None" || weather == "DustClouds")
            {
                weatherModifier = 0.75f;
            }
            else
            {
                weatherModifier = 0.9f;
            }
            return weatherModifier;
        }
    }
}
