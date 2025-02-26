using HarmonyLib;
using LethalLevelLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DynamicMoonRatings.Patches
{
    [HarmonyPatch(typeof(LevelManager))]
    public class LevelManagerMoonRatingsPatch
    {
        [HarmonyPatch(nameof(LevelManager.CalculateExtendedLevelDifficultyRating))]
        [HarmonyPostfix]
        public static void CalculateExtendedLevelDifficultyRatingPostfix(ref int __result, ExtendedLevel extendedLevel, bool debugResults = false)
        {
            //Plugin.Logger.LogError("LevelManager CalculateExtendedLevelDifficultyRating Starting");
            __result = Modules.RatingsCalculation.CalculateDifficulty(extendedLevel);
            return;
        }
    }

    public class DiffCalc
    {
        public static int originalCalc(ExtendedLevel level)
        {
            int returnRating = 0;

            SelectableLevel sl = level.SelectableLevel;

            //basic (route price + max scrap
            int baselineRouteValue = level.RoutePrice;
            baselineRouteValue += sl.maxTotalScrapValue;
            returnRating += baselineRouteValue;

            //scrap
            int scrapValue = 0;
            foreach (SpawnableItemWithRarity spawnableScrap in sl.spawnableScrap)
            {
                if (spawnableScrap.spawnableItem != null)
                {
                    if (((spawnableScrap.spawnableItem.minValue + spawnableScrap.spawnableItem.maxValue) * 5) != 0 && spawnableScrap.rarity != 0)
                    {
                        if ((spawnableScrap.rarity / 10) != 0)
                            scrapValue += (spawnableScrap.spawnableItem.maxValue - spawnableScrap.spawnableItem.minValue) / (spawnableScrap.rarity / 10);
                    }
                }
            }
            returnRating += scrapValue;

            //enemies basic
            int enemySpawnValue = (sl.maxEnemyPowerCount + sl.maxOutsideEnemyPowerCount + sl.maxDaytimeEnemyPowerCount) * 15;
            enemySpawnValue = enemySpawnValue * 2;
            returnRating += enemySpawnValue;

            //enemies rarity
            float enemyValue = 0;
            foreach (SpawnableEnemyWithRarity spawnableEnemy in sl.Enemies.Concat(sl.OutsideEnemies).Concat(sl.DaytimeEnemies))
            {
                if (spawnableEnemy.rarity != 0 && spawnableEnemy.enemyType != null)
                    if ((spawnableEnemy.rarity / 10) != 0)
                        enemyValue += (spawnableEnemy.enemyType.PowerLevel * 100) / (spawnableEnemy.rarity / 10);
            }
            returnRating += Mathf.RoundToInt(enemyValue);


            //incremental
            returnRating += Mathf.RoundToInt(returnRating * (sl.factorySizeMultiplier * 0.5f));

            Plugin.Logger.LogError("Moon=" + level.name + "|Rating=" + returnRating);
            string displayedRating = (returnRating / 10).ToString();
            sl.riskLevel = displayedRating.ToString();
            return returnRating;
        }

        public static int newCalc(ExtendedLevel level)
        {
            int returnRating = 0;

            //basic (route price + max scrap
            int baselineRouteValue = level.RoutePrice;
            baselineRouteValue += level.SelectableLevel.maxTotalScrapValue;
            returnRating += baselineRouteValue;

            //scrap
            int scrapValue = 0;
            foreach (SpawnableItemWithRarity spawnableScrap in level.SelectableLevel.spawnableScrap)
            {
                if (spawnableScrap.spawnableItem != null)
                {
                    if (((spawnableScrap.spawnableItem.minValue + spawnableScrap.spawnableItem.maxValue) * 5) != 0 && spawnableScrap.rarity != 0)
                    {
                        if ((spawnableScrap.rarity / 10) != 0)
                            scrapValue += (spawnableScrap.spawnableItem.maxValue - spawnableScrap.spawnableItem.minValue) / (spawnableScrap.rarity / 10);
                    }
                }
            }
            returnRating += scrapValue;

            //enemies basic
            int enemySpawnValue = (level.SelectableLevel.maxEnemyPowerCount + level.SelectableLevel.maxOutsideEnemyPowerCount + level.SelectableLevel.maxDaytimeEnemyPowerCount) * 15;
            enemySpawnValue = enemySpawnValue * 2;
            returnRating += enemySpawnValue;

            //enemies rarity
            float enemyValue = 0;
            foreach (SpawnableEnemyWithRarity spawnableEnemy in level.SelectableLevel.Enemies.Concat(level.SelectableLevel.OutsideEnemies).Concat(level.SelectableLevel.DaytimeEnemies))
            {
                if (spawnableEnemy.rarity != 0 && spawnableEnemy.enemyType != null)
                    if ((spawnableEnemy.rarity / 10) != 0)
                        enemyValue += (spawnableEnemy.enemyType.PowerLevel * 100) / (spawnableEnemy.rarity / 10);
            }
            returnRating += Mathf.RoundToInt(enemyValue);

            //WEATHER MODIFIERS
            string weather = level.SelectableLevel.currentWeather.ToString();
            float weatherModifier = 1f;
            if (weather == "Eclipsed")
            {
                weatherModifier = 1.2f;
            }
            else if (weather == "Flooded" || weather == "Stormy")
            {
                weatherModifier = 1.1f;
            }
            else if (weather == "Rainy")
            {
                weatherModifier = 0.9f;
            }
            else if (weather == "None" || weather == "DustClouds")
            {
                weatherModifier = 0.8f;
            }
            else
            {
                weatherModifier = 1f;
            }

            returnRating = (int)(returnRating * weatherModifier);

            //CRUISER ADDITIONS
            float mentionsCruiserModifier;
            if (level.SelectableLevel.LevelDescription.Contains("Cruiser")) //is surely a cruiser moon so "add" difficulty points...
            {
                mentionsCruiserModifier = 1.25f;
            }
            else
            {
                mentionsCruiserModifier = 1f;
            }
            returnRating = (int)(returnRating * mentionsCruiserModifier);

            //incremental
            returnRating += Mathf.RoundToInt(returnRating * (level.SelectableLevel.factorySizeMultiplier * 0.35f)); //the only REAL change is 0.025f instead of 0.5f

            Plugin.Logger.LogError("Moon=" + level.name + "|Rating=" + returnRating);
            string displayedRating = (returnRating / 10).ToString();
            level.SelectableLevel.riskLevel = displayedRating.ToString();
            return returnRating;
        }

        public static int recalculateDifficulty(ExtendedLevel level)
        {
            SelectableLevel sL = level.SelectableLevel;
            Plugin.Logger.LogError("Moon Name = " + sL.ToString());

            // CRUISER MODIFIERS
            float mentionsCruiserModifier;
            if (sL.LevelDescription.Contains("Cruiser")) //is surely a cruiser moon so "add" difficulty points...
            {
                mentionsCruiserModifier = 1.25f;
            }
            else
            {
                mentionsCruiserModifier = 1f;
            }
            Plugin.Logger.LogWarning("CruiserRatingChange = " + mentionsCruiserModifier.ToString());

            //SCRAP MODIFIERS
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
            Plugin.Logger.LogWarning("ScrapRatingChange = " + scrapAverageModifier.ToString());

            //DAY SPEED MODIFIERS
            float DaySpeedMultiplierModifier = sL.DaySpeedMultiplier;

            Plugin.Logger.LogWarning("DaySpeedMultiplierRatingChange = " + DaySpeedMultiplierModifier.ToString());

            //ENEMY MODIFIERS
            int maxDaytimeEnemyPowerCount = sL.maxDaytimeEnemyPowerCount; //outside daytime?
            int maxOutsideEnemyPowerCount = sL.maxOutsideEnemyPowerCount; //outside nighttime?
            int maxEnemyPowerCount = sL.maxEnemyPowerCount; //inside enemies?
            var dayTimeEnemies = sL.DaytimeEnemies; //outside daytime?
            var outsideEnemies = sL.OutsideEnemies; //outside nighttime?
            var enemies = sL.Enemies; //inside enemies?
            float enemiesModifier = checkEnemiesForAddedDifficulty(dayTimeEnemies, outsideEnemies, enemies);

            Plugin.Logger.LogWarning("EnemyRatingChange = " + enemiesModifier.ToString());

            //PRICE MODIFIERS
            int routePrice = level.RoutePrice;
            float priceModifier;
            /*if (routePrice > 0)
            {
                priceModifier = 1 + (routePrice / 100);
            }
            else
            {
                priceModifier = 1;
            }*/
            priceModifier = (routePrice / 10);

            Plugin.Logger.LogWarning("PriceRatingChange = " + priceModifier.ToString());

            //WEATHER MODIFIERS
            string weather = sL.currentWeather.ToString();
            float weatherModifier = 1f;
            if (weather == "Eclipsed")
            {
                weatherModifier = 1.2f;
            }
            else if (weather == "Flooded" || weather == "Stormy")
            {
                weatherModifier = 1.1f;
            }
            else if (weather == "Rainy")
            {
                weatherModifier = 0.9f;
            }
            else if (weather == "None" || weather == "DustClouds")
            {
                weatherModifier = 0.8f;
            }
            else
            {
                weatherModifier = 1f;
            }

            Plugin.Logger.LogWarning("WeatherRatingChange = " + weatherModifier.ToString());

            //RESULT CALCULATION
            //rating is -10% day length
            //rating is 20% weather
            //rating is 20% enemies
            //rating is -20% scrap amount
            //rating is 20% moon price
            //rating is 10% cruiser

            float result = 0;
            /*if(true) //day length
            {
                result = result - (DaySpeedMultiplierModifier * 10);
            }
            if(true) //scrap
            {
                result = result - (scrapAverageModifier * 20);
            }
            if(true) //enemies
            {
                result = result + (enemiesModifier * 20);
            }
            if(true) //weather
            {
                result = result + (weatherModifier * 20);
            }
            if(true) //price
            {
                result = result + (priceModifier * 20);
            }
            if(true) //cruiser
            {
                result = result + (mentionsCruiserModifier * 10);
            }*/

            //new result calculation
            result = result + ((enemiesModifier + priceModifier) - scrapAverageModifier); //enemies - (scrap + price balance)
            result = result * (DaySpeedMultiplierModifier * 100);
            result = result * (weatherModifier * 100);
            result = result * (mentionsCruiserModifier * 100);

            Plugin.Logger.LogInfo("Result!=" + result);
            string rLetter = string.Empty;
            string rNumber = string.Empty;
            if (result >= 0 && result <= 99)
            {
                rLetter = "J";
                rNumber = result.ToString("00");
            }
            else if (result >= 100 && result <= 199)
            {
                rLetter = "I";
                rNumber = (-100 + result).ToString("00");
            }
            else if (result >= 200 && result <= 299)
            {
                rLetter = "H";
                rNumber = (-200 + result).ToString("00");
            }
            else if (result >= 300 && result <= 399)
            {
                rLetter = "G";
                rNumber = (-300 + result).ToString("00");
            }
            else if (result >= 400 && result <= 499)
            {
                rLetter = "F";
                rNumber = (-400 + result).ToString("00");
            }
            else if (result >= 500 && result <= 599)
            {
                rLetter = "E";
                rNumber = (-500 + result).ToString("00");
            }
            else if (result >= 600 && result <= 699)
            {
                rLetter = "D";
                rNumber = (-600 + result).ToString("00");
            }
            else if (result >= 700 && result <= 799)
            {
                rLetter = "C";
                rNumber = (-700 + result).ToString("00");
            }
            else if (result >= 800 && result <= 899)
            {
                rLetter = "B";
                rNumber = (-800 + result).ToString("00");
            }
            else if (result >= 900 && result <= 999)
            {
                rLetter = "S";
                rNumber = (-900 + result).ToString("00");
            }
            else if (result < 0)
            {
                rLetter = "J";
                rNumber = "--";
            }
            else
            {
                rLetter = "S";
                rNumber = "++";
            }

            sL.riskLevel = result.ToString("000");
            //sL.riskLevel = string.Format("{0}{1}",rLetter,rNumber);
            Plugin.Logger.LogWarning("result = " + string.Format("{0}{1}", rLetter, rNumber));
            return (int)result;
        }

        private static float checkEnemiesForAddedDifficulty(List<SpawnableEnemyWithRarity> day, List<SpawnableEnemyWithRarity> night, List<SpawnableEnemyWithRarity> inside)
        {
            List<SpawnableEnemyWithRarity> fullList = new List<SpawnableEnemyWithRarity>();
            fullList.AddRange(day);
            fullList.AddRange(night);
            fullList.AddRange(inside);
            float enemyDifficultyModifier = 1f;
            foreach (SpawnableEnemyWithRarity enemy in fullList)
            {
                Plugin.Logger.LogDebug("Enemy = " + enemy.enemyType.name);
                string n = enemy.enemyType.enemyName;
                switch (n)
                {
                    case "SpringMan":
                    case "ClaySurgeon":
                        enemyDifficultyModifier += (1.50f * enemy.rarity);
                        break;
                    case "Nutcracker":
                    case "RadMech":
                    case "DressGirl":
                    case "CaveDweller":
                    case "Jester":
                        enemyDifficultyModifier += (1.25f * enemy.rarity);
                        break;
                    case "MaskedPlayerEnemy":
                    case "Flowerman":
                    case "MouthDog":
                    case "ForestGiant":
                    case "SandWorm":
                    case "Crawler":
                    case "SandSpider":
                    case "Butler":
                        enemyDifficultyModifier += (1.10f * enemy.rarity);
                        break;
                    default:
                        enemyDifficultyModifier += (1f * enemy.rarity);
                        break;
                }
            }
            return (enemyDifficultyModifier / 1.5f);
        }
    }
}
