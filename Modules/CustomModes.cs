using BepInEx.Configuration;
using LethalLevelLoader;
using LethalLevelLoader.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.InputSystem.Utilities;

namespace DynamicMoonRatings.Modules
{
    internal class CustomTier
    {
        internal string uniqueName;
        internal int minRating;
        internal string displayName;
    }

    internal class CustomRating
    {
        internal string uniqueName;
        internal int rating;
        internal string displayName;
    }

    internal class CustomModes
    {
        internal static List<ExtendedLevel> moons = LethalLevelLoader.PatchedContent.ExtendedLevels;

        internal static int GetCustomRating(ExtendedLevel level) //not used anymore
        {
            int rating = Int16.MinValue;
            if (Plugin.customRatings.Any(x => x.uniqueName == level.UniqueIdentificationName))
            {
                rating = Plugin.customRatings.Find(x => x.uniqueName == level.UniqueIdentificationName).rating;
            }
            Plugin.Logger.LogDebug(level.NumberlessPlanetName.ToString() + " custom rating is set: " + rating.ToString());
            level.SelectableLevel.riskLevel = updateCustomDisplayedString(rating);
            return rating; //nothing done with this yet, config requiring setup first.
        }

        internal static string updateCustomDisplayedString(int rating)
        {
            string displayRating = "error";
            foreach (var ratingLevel in Plugin.customTiers.OrderBy(x => x.minRating))
            {
                if (rating >= ratingLevel.minRating)
                {
                    displayRating = ratingLevel.displayName;
                }
            }
            if (Plugin.displayMode == "A###")
            {
                int modifiedRating = (int)(rating - (1000 * (int)Math.Floor((float)(rating / 1000))));
                Plugin.Logger.LogDebug(Plugin.displayMode + " CustomDisplayString Set: " + displayRating + (modifiedRating).ToString("000"));
                return displayRating + (modifiedRating).ToString("000");
            }
            else if (Plugin.displayMode == "A")
            {
                Plugin.Logger.LogDebug(Plugin.displayMode + " CustomDisplayString Set: " + displayRating);
                return displayRating;
            }
            else //####
            {
                Plugin.Logger.LogDebug(Plugin.displayMode + " CustomDisplayString Set: " + rating.ToString("0000"));
                return rating.ToString("0000");
            }
        }
        /*internal static List<string, int> GetCustomTiers()
        {

            return null;
        }*/
    }
}
