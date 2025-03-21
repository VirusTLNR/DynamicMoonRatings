using LethalLevelLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicMoonRatings.Modules
{
    public static class Extensions
    {
        [Obsolete] //no longer needed as the issue is bypassed, also this doesnt really work
        public static int SetNearestAvailableRating(this Dictionary<int, string> dict, ExtendedLevel level, int rating)
        {
            int modifiedRating = rating;
            int finalRating = int.MinValue; //this value should NEVER be used.
            for (int i = 653; i < 675; i++)
            {
                if (dict.TryGetValue(i, out string value))
                    Plugin.Logger.LogWarning(value + ":" + i);
                else
                    Plugin.Logger.LogWarning("no value:" + i);
            }
            if (dict.ContainsValue(level.UniqueIdentificationName))
            {
                dict.Remove(dict.First(x => x.Value == level.UniqueIdentificationName).Key); //remove because it will be re-added within this method.
            }
            if (!dict.TryGetValue(modifiedRating, out string val1))
            {
                finalRating = modifiedRating; //actual rating is used
                Plugin.Logger.LogInfo(level.UniqueIdentificationName + " rating is unmodified: " + finalRating);
            }
            else
            {
                modifiedRating = Enumerable.Range(rating - 1, 3).Except(dict.Keys).FirstOrDefault();
                if (!dict.TryGetValue(modifiedRating, out string val2))
                {
                    finalRating = modifiedRating;
                }
                else
                {
                    modifiedRating = Enumerable.Range(rating - 2, 5).Except(dict.Keys).FirstOrDefault();
                    if (!dict.TryGetValue(modifiedRating, out string val3))
                    {
                        finalRating = modifiedRating;
                    }
                    else
                    {
                        modifiedRating = Enumerable.Range(rating - 22, 50).Except(dict.Keys).FirstOrDefault();
                        if (!dict.TryGetValue(modifiedRating, out string val4))
                        {
                            finalRating = modifiedRating;
                        }
                        else
                        {
                            for (int r = -6000; r < -4000; r++)
                            {
                                if (!dict.TryGetValue(r, out string val5))
                                {
                                    modifiedRating = r;
                                    break;
                                }
                            }
                        }
                    }
                }
                Plugin.Logger.LogInfo(level.UniqueIdentificationName + " rating is modified due to conflict with another moon: " + finalRating + "(original rating: " + rating + ").");

            }
            dict.Add(finalRating, level.UniqueIdentificationName);

            for (int i = 653; i < 675; i++)
            {
                if (dict.TryGetValue(i, out string value))
                    Plugin.Logger.LogWarning(value + ":" + i);
                else
                    Plugin.Logger.LogWarning("no value:" + i);
            }
            /*int firstTry = Enumerable.Range(rating - 1, 3).Except(dict.Keys).FirstOrDefault();
            Plugin.Logger.LogError(level.UniqueIdentificationName + " Jiggling1: " + firstTry);
            int secondTry = Enumerable.Range(rating - 2, 5).Except(dict.Keys).FirstOrDefault();
            Plugin.Logger.LogError(level.UniqueIdentificationName + " Jiggling2: " + secondTry);
            int thirdTry = Enumerable.Range(rating - 10, 21).Except(dict.Keys).FirstOrDefault();
            Plugin.Logger.LogError(level.UniqueIdentificationName + " Jiggling3: " + thirdTry);*/
            return finalRating;
        }
    }
}
