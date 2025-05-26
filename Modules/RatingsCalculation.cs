using LethalLevelLoader;
using DynamicMoonRatings.Modules;
using System;
using System.Collections.Generic;
using System.Text;
using DynamicMoonRatings.Modules.CalculationsLLL;
using DynamicMoonRatings.Modules.CalculationsV1;

namespace DynamicMoonRatings.Modules
{
    internal static class RatingsCalculation
    {
        internal static int RatingSelector(ExtendedLevel level)
        {
            int newRating = -1; //for incase the rating calculation fails so we can revert back to the original rating //not implemented yet
            switch(Plugin.ratingsVersion)
            {
                case "CUSTOM":
                    //newRating = CustomModes.GetCustomRating(level); //this should never occur anymore as the rating is set at config load
                    break;
                case "LLL":
                    newRating = CalculateDifficultyLLL(level); 
                    break;
                case "V1":
                    newRating = CalculateDifficultyV1(level);
                    break;
            }
            return newRating;
        }

        internal static int CalculateDifficultyLLL(ExtendedLevel level)
        {
            int tempResult = LLL.CalculateSameAsLLL(level);
            int result = (int)(float)(tempResult * 3.5f);
            string displayResult = ModifyDisplayedValue(result);
            Plugin.Logger.LogDebug(
                "[Moon]" + level.SelectableLevel.PlanetName.ToString() +
                "       = [LLLRating]" + tempResult +
                "       = [ModTotal]" + result +
                "       = [DisplayedRating]" + displayResult
                );
            //Plugin.Logger.LogInfo("TerminalDisplayResult = " + displayResult);
            level.SelectableLevel.riskLevel = displayResult;
            return (int)result;
        }

        internal static int CalculateDifficultyV1(ExtendedLevel level)
        {
            string moon = level.SelectableLevel.PlanetName.ToString();
            //Plugin.Logger.LogError("Moon Name = " + moon);
            float enemies = CalculationsV1.Enemies.EnemiesCalc(level) * 5f;
            //Plugin.Logger.LogWarning("Enemies = +" + enemies);
            float price = CalculationsV1.Price.PriceCalc(level) * 2f;
            //Plugin.Logger.LogWarning("Price = +" + price);
            float cruiser = CalculationsV1.Cruiser.CruiserCalc(level) * 2f;
            //Plugin.Logger.LogWarning("Cruiser = +" + cruiser);
            float scrap = (Plugin.usageMode=="Risk Mode"? CalculationsV1.Scrap.ScrapCalc(level) * 30f:1000); //risk mode = calculation, difficulty mode = 1000
            //Plugin.Logger.LogWarning("Scrap = -" + scrap);
            float weather = CalculationsV1.Weather.WeatherCalc(level);
            //Plugin.Logger.LogWarning("Weather = x" + weather);
            float time = CalculationsV1.Time.TimeCalc(level);
            //Plugin.Logger.LogWarning("Time = /" + time);

            float result = 0;
            //result = result + (((enemies * 0.25f) + (price * 0.1f) + (cruiser * 0.1f)) - (scrap * 1.5f)); //enemies - (scrap + price balance)
            result = result + ((enemies + price + cruiser) - scrap); //enemies - (scrap + price balance)
            result = result * (weather);
            result = result / (time);

            //Plugin.Logger.LogInfo("SortingResult = " + result.ToString("00"));
            string displayResult = ModifyDisplayedValue(result);

            Plugin.Logger.LogDebug(
                "[Moon]" + moon +
                "           -> (([Enemies]" + enemies +
                "       + [Price]" + price +
                "       + [Cruiser]" + cruiser +
                ")      - [Scrap]" + scrap +
                ")      x [Weather]" + weather +
                "       / [Time]" + time +
                "       = [Total]" + result +
                "       = [DisplayedRating]" + displayResult
                //(Plugin.usageMode != "Risk Mode" ?"       = [ScrapGen]" + ScrapGeneration.ScrapGenSelector(RoundManager.Instance):"") //cant be done as it causes errors
                );
            //Plugin.Logger.LogInfo("TerminalDisplayResult = " + displayResult);
            level.SelectableLevel.riskLevel = displayResult;
            return (int)result;
        }

        internal static string ModifyDisplayedValue(float rating)
        {
            string n = Plugin.displayMode; //0 = letter+num, 1 = letter, 2 = num
            string rLetter = string.Empty;
            string rNumber = string.Empty;
            float displayRating = rating / 10;
            int modifiedRating = (int)(displayRating - (1000 * (int)Math.Floor((decimal)(displayRating / 1000))));
            if (n.Contains("A"))
            {
                if (displayRating >= 0 && displayRating <= 999)
                {
                    rLetter = "F";
                    rNumber = (modifiedRating).ToString("000");
                }
                else if (displayRating >= 1000 && displayRating <= 1999)
                {
                    rLetter = "E";
                    rNumber = (modifiedRating).ToString("000");
                }
                else if (displayRating >= 2000 && displayRating <= 2999)
                {
                    rLetter = "D";
                    rNumber = (modifiedRating).ToString("000");
                }
                else if (displayRating >= 3000 && displayRating <= 3999)
                {
                    rLetter = "C";
                    rNumber = (modifiedRating).ToString("000");
                }
                else if (displayRating >= 4000 && displayRating <= 4999)
                {
                    rLetter = "B";
                    rNumber = (modifiedRating).ToString("000");
                }
                else if (displayRating >= 5000 && displayRating <= 5999)
                {
                    rLetter = "A";
                    rNumber = (modifiedRating).ToString("000");
                }
                else if (displayRating >= 6000 && displayRating <= 6999)
                {
                    rLetter = "S";
                    rNumber = (modifiedRating).ToString("000");
                }
                else if(displayRating >= 7000)
                {
                    rLetter = "S";
                    rNumber = "+";
                }
                else if(displayRating <= 0)
                {
                    rLetter = "F";
                    rNumber = "-";
                }
                //if (displayRating >= 0 && displayRating <= 99)
                //{
                //    rLetter = "J";
                //    rNumber = displayRating.ToString("00");
                //}
                //else if (displayRating >= 100 && displayRating <= 199)
                //{
                //    rLetter = "I";
                //    rNumber = (-100 + displayRating).ToString("00");
                //}
                //else if (displayRating >= 200 && displayRating <= 299)
                //{
                //    rLetter = "H";
                //    rNumber = (-200 + displayRating).ToString("00");
                //}
                //else if (displayRating >= 300 && displayRating <= 399)
                //{
                //    rLetter = "G";
                //    rNumber = (-300 + displayRating).ToString("00");
                //}
                //else if (displayRating >= 400 && displayRating <= 499)
                //{
                //    rLetter = "D";
                //    rNumber = (-400 + displayRating).ToString("00");
                //}
                //else if (displayRating >= 500 && displayRating <= 599)
                //{
                //    rLetter = "C";
                //    rNumber = (-500 + displayRating).ToString("00");
                //}
                //else if (displayRating >= 600 && displayRating <= 699)
                //{
                //    rLetter = "B";
                //    rNumber = (-600 + displayRating).ToString("00");
                //}
                //else if (displayRating >= 700 && displayRating <= 799)
                //{
                //    rLetter = "A";
                //    rNumber = (-700 + displayRating).ToString("00");
                //}
                //else if (displayRating >= 800 && displayRating <= 899)
                //{
                //    rLetter = "S";
                //    rNumber = (-800 + displayRating).ToString("00");
                //}
                //else if (displayRating >= 900 && displayRating <= 999)
                //{
                //    rLetter = "S";
                //    rNumber = (-900 + displayRating).ToString("00");
                //}
                //else if (displayRating < 0)
                //{
                //    rLetter = "E";
                //    rNumber = "-";
                //}
                //else
                //{
                //    rLetter = "S";
                //    rNumber = "+";
                //}
            }
            else
            {
                rNumber = displayRating.ToString("0000");
            }
            string outputValue = "#ERR#";
            switch (n)
            {
                case "A###":
                    //speed run mode = L## format
                    outputValue = string.Format("{0}{1}", rLetter, rNumber);
                    break;
                case "A":
                    //letter only
                    outputValue = string.Format("{0}", rLetter);
                    break;
                case "####":
                    //number only
                    outputValue = string.Format("{0}", rNumber);
                    break;
            }

            return outputValue;
        }
    }
}
