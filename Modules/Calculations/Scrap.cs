using LethalLevelLoader;

namespace DynamicMoonRatings.Modules.Calculations
{
    internal class Scrap
    {
        internal static float ScrapCalc(ExtendedLevel level)
        {
            SelectableLevel sL = level.SelectableLevel;
            int minScrap = sL.minScrap;
            int maxScrap = sL.maxScrap;
            int minTotalScrapValue = sL.minTotalScrapValue;
            int maxTotalScrapValue = sL.maxTotalScrapValue;
            float scrapMinMinModifier = minTotalScrapValue / minScrap;
            float scrapMinMaxModifier = minTotalScrapValue / maxScrap;
            float scrapMaxMinModifier = maxTotalScrapValue / minScrap;
            float scrapMaxMaxModifier = maxTotalScrapValue / maxScrap;

            float scrapAverageModifier = ((scrapMinMinModifier + scrapMinMaxModifier + scrapMaxMinModifier + scrapMaxMaxModifier) / 4);
            float scrapAverageTotal = (maxTotalScrapValue - minTotalScrapValue);
            float scrapAverageItems = (maxScrap - minScrap);
            float factorySizeMultiplier = sL.factorySizeMultiplier;

            //total scrap divided by number of items, divided by rough distance
            float scrapBaseLine = (scrapAverageTotal / scrapAverageItems / factorySizeMultiplier);
            //float scrapBaseLine = scrapAverageModifier;

            return scrapBaseLine;
        }
    }
}
