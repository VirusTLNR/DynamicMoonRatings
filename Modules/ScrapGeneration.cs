using LethalLevelLoader;
using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicMoonRatings.Modules
{
    internal class ScrapGeneration
    {
        internal static void ScrapGenSelector(RoundManager __instance, bool generateScrapNow = false)
        {
            switch (Plugin.scrapGenVersion)
            {
                case "V1":
                    scrapGenV1(__instance, generateScrapNow);
                    break;
            }
        }

        private static void scrapGenV1(RoundManager __instance, bool generateScrapNow = false)
        {
            TimeOfDay tod = TimeOfDay.Instance;
            StartOfRound sor = StartOfRound.Instance;

            /*if (__instance == null || tod == null || sor == null)
            {
                return null;
            }*/

            //quota modifier
            float quotaValue = tod.profitQuota;
            float quotaNumber = (int)(quotaValue / 50); //for every 50 quota, add 1% to quota multiplier

            //day modifier
            float dayNumber = sor.gameStats.daysSpent; //for every day in the game, add 1% to day multiplier

            //rating value
            float origRating = Modules.RatingsCalculation.RatingSelector(LevelManager.GetExtendedLevel(__instance.currentLevel)); //CHANGED TO USE THE RATINGS VERSION SELECTOR.. POTENTIALLY DIFFERENT SCRAP GEN VERSION AND RATING VERSION MAY CAUSE UNINTENDED EFFECTS

            //compatibility with CUSTOM ratings...
            float rating = (Plugin.ratingsVersion == "CUSTOM" ? 10 : 1) * origRating; //10 x for CUSTOM, 1 x for others

            if (rating < 0) rating = 0; //cap at 0 (- not allowed)
            else if (rating > 70000) rating = 70000; //cap at 70000

            float ratingNumber = (int)(rating / 200); //for every 200 rating, add 1% to rating multiplier

            //moon mod
            float moonMod = ((100 + (ratingNumber + quotaNumber + dayNumber)) / 100); //divide rating by 100 to get the rating as a percentage.


            Plugin.Logger.LogDebug("Moon: " + __instance.currentLevel.PlanetName + " | Rating: " + rating.ToString() + " -> Rating%: " + ratingNumber + " | Day%: " + dayNumber + " | Quota%: " + quotaNumber);
            Plugin.Logger.LogDebug("MoonModifier ((100%+(Rating%+Days%+Quota%))/100) -> " + moonMod.ToString());

            int minScrapVal = (int)(moonMod * Plugin.difficultyModeMinScrapMod);
            int maxScrapVal = (int)(moonMod * Plugin.difficultyModeMaxScrapMod);
            int minScrapTot = (int)(moonMod * Plugin.difficultyModeMinScrapMod) / 35;
            int MaxScrapTot = (int)(moonMod * Plugin.difficultyModeMaxScrapMod) / 35;

            if (generateScrapNow)
            {
                __instance.currentLevel.minTotalScrapValue = minScrapVal;
                __instance.currentLevel.maxTotalScrapValue = maxScrapVal;
                __instance.currentLevel.minScrap = minScrapTot;
                __instance.currentLevel.maxScrap = MaxScrapTot;
            }
            Plugin.Logger.LogDebug("ScrapValue -> " + minScrapVal + " - " + maxScrapVal);
            Plugin.Logger.LogDebug("ScrapCount -> " + minScrapTot + " - " + MaxScrapTot);

            //return minScrapVal + " - " + maxScrapVal + "(" + minScrapTot + " - " + MaxScrapTot + ")";
        }
    }
}
