using LethalLevelLoader;

namespace DynamicMoonRatings.Modules.CalculationsV1
{
    internal class Weather
    {
        internal static float WeatherCalc(ExtendedLevel level)
        {
            string weather = level.SelectableLevel.currentWeather.ToString();
            float weatherModifier = 1f;
            if (weather == "Eclipsed") //tier 5 weather
            {
                weatherModifier = 1.2f;
            }
            else if (weather == "Flooded" || weather == "Stormy") //tier 4 weather
            {
                weatherModifier = 1.1f;
            }
            else if (weather == "Rainy") //tier 3 weather
            {
                weatherModifier = 1f;
            }
            else if (weather == "None" || weather == "DustClouds") //tier 1 weather
            {
                weatherModifier = 0.8f;
            }
            else //tier 2 weather
            {
                weatherModifier = 0.9f; 
            }
            return weatherModifier;
        }
    }
}
