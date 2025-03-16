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
        private static Dictionary<int, string> currentMoonRatingsList = new Dictionary<int, string>();

        [HarmonyPatch(nameof(LevelManager.CalculateExtendedLevelDifficultyRating))]
        [HarmonyPostfix]
        public static void CalculateExtendedLevelDifficultyRatingPostfix(ref int __result, ExtendedLevel extendedLevel, bool debugResults = false)
        {
            int updatedRating = Modules.RatingsCalculation.RatingSelector(extendedLevel);
            //updatedRating = Modules.Extensions.SetNearestAvailableRating(currentMoonRatingsList, extendedLevel, updatedRating);
            if (updatedRating != -1) //if the rating calculation fails, it will use the original rating.
            {
                __result = updatedRating;
            }
            //__result = Modules.RatingsCalculation.CalculateDifficultyV1(extendedLevel);
            return;
        }

        /*[HarmonyPatch(nameof(LevelManager.PopulateDynamicRiskLevelDictionary))] //maybe i dont want to do this, and instead adapt it to my own things.
        [HarmonyPrefix]
        public static bool RiskDictionaryPopulationPrefix()
        {
            Dictionary<string, List<int>> vanillaRiskLevelDictionary = new Dictionary<string, List<int>>();

            foreach (ExtendedLevel vanillaLevel in PatchedContent.VanillaExtendedLevels)
            {
                //DebugHelper.Log("Risk Level Of " + vanillaLevel.NumberlessPlanetName + " Is: " + vanillaLevel.SelectableLevel.riskLevel, DebugType.Developer);
                if (!vanillaLevel.SelectableLevel.riskLevel.Contains("Safe") && !string.IsNullOrEmpty(vanillaLevel.SelectableLevel.riskLevel))
                {
                    if (vanillaRiskLevelDictionary.TryGetValue(vanillaLevel.SelectableLevel.riskLevel, out List<int> dynamicDifficultyRatingList))
                        dynamicDifficultyRatingList.Add(vanillaLevel.CalculatedDifficultyRating);
                    else
                        vanillaRiskLevelDictionary.Add(vanillaLevel.SelectableLevel.riskLevel, new List<int>() { vanillaLevel.CalculatedDifficultyRating });
                }
            }

            foreach (KeyValuePair<string, List<int>> vanillaRiskLevel in vanillaRiskLevelDictionary)
            {
                string debugString = "Vanilla Risk Level Group (" + vanillaRiskLevel.Key + "): ";
                if (vanillaRiskLevel.Value != null)
                {
                    debugString += " Average - " + vanillaRiskLevel.Value.Average() + ", Values - ";
                    foreach (int calculatedDifficulty in vanillaRiskLevel.Value)
                        debugString += calculatedDifficulty.ToString() + ", ";
                }
                //DebugHelper.Log(debugString, DebugType.Developer);
            }

            foreach (KeyValuePair<string, int> dynamicRiskLevelPair in new Dictionary<string, int>(LevelManager.dynamicRiskLevelDictionary))
                foreach (KeyValuePair<string, List<int>> vanillaRiskLevel in vanillaRiskLevelDictionary)
                    if (dynamicRiskLevelPair.Key.Equals(vanillaRiskLevel.Key))
                    {
                        //DebugHelper.Log("Setting RiskLevel " + vanillaRiskLevel.Key + " To " + (int)vanillaRiskLevel.Value.Average(), DebugType.Developer);
                        LevelManager.dynamicRiskLevelDictionary[dynamicRiskLevelPair.Key] = Mathf.RoundToInt((float)vanillaRiskLevel.Value.Average());
                    }

            //DebugHelper.Log("Starting To Assign - and + Risk Levels", DebugType.Developer);
            int counter = 0;
            foreach (KeyValuePair<string, int> dynamicRiskLevelPair in new Dictionary<string, int>(LevelManager.dynamicRiskLevelDictionary))
            {
                string previousFullRiskLevel = string.Empty;
                string currentFullRiskLevel = string.Empty;
                string nextFullRiskLevel = string.Empty;
                //DebugHelper.Log("Trying To Assign Value To Risk Level: " + dynamicRiskLevelPair.Key, DebugType.Developer);

                if (dynamicRiskLevelPair.Key.Contains("-"))
                {
                    if (counter != 0)
                        previousFullRiskLevel = LevelManager.dynamicRiskLevelDictionary.Keys.ToList()[counter - 2];
                    currentFullRiskLevel = LevelManager.dynamicRiskLevelDictionary.Keys.ToList()[counter + 1];

                    if (counter == 0)
                        LevelManager.dynamicRiskLevelDictionary[dynamicRiskLevelPair.Key] = (LevelManager.dynamicRiskLevelDictionary[currentFullRiskLevel] / 2);
                    else
                        LevelManager.dynamicRiskLevelDictionary[dynamicRiskLevelPair.Key] = Mathf.RoundToInt(Mathf.Lerp(LevelManager.dynamicRiskLevelDictionary[previousFullRiskLevel], LevelManager.dynamicRiskLevelDictionary[currentFullRiskLevel], 0.66f));

                }
                else if (dynamicRiskLevelPair.Key.Contains("+") && !dynamicRiskLevelPair.Key.Equals("S+"))
                {
                    currentFullRiskLevel = LevelManager.dynamicRiskLevelDictionary.Keys.ToList()[counter - 1];
                    if (!dynamicRiskLevelPair.Key.Contains("S"))
                        nextFullRiskLevel = LevelManager.dynamicRiskLevelDictionary.Keys.ToList()[counter + 2];

                    if (dynamicRiskLevelPair.Key.Equals("S++"))
                        LevelManager.dynamicRiskLevelDictionary[dynamicRiskLevelPair.Key] = (LevelManager.dynamicRiskLevelDictionary[currentFullRiskLevel] * 2);
                    else if (dynamicRiskLevelPair.Key.Equals("S+++"))
                        LevelManager.dynamicRiskLevelDictionary[dynamicRiskLevelPair.Key] = (LevelManager.dynamicRiskLevelDictionary[currentFullRiskLevel] * 3);
                    else
                        LevelManager.dynamicRiskLevelDictionary[dynamicRiskLevelPair.Key] = Mathf.RoundToInt(Mathf.Lerp(LevelManager.dynamicRiskLevelDictionary[currentFullRiskLevel], LevelManager.dynamicRiskLevelDictionary[nextFullRiskLevel], 0.33f));
                }

                //DebugHelper.Log("Risk Level: " + dynamicRiskLevelPair.Key + " Was Assigned Calculated Difficulty Of: " + dynamicRiskLevelDictionary[dynamicRiskLevelPair.Key].ToString(), DebugType.Developer);
                counter++;
            }

            //foreach (KeyValuePair<string, int> dynamicRiskLevelPair in new Dictionary<string, int>(dynamicRiskLevelDictionary))
            //    DebugHelper.Log("Dynamic Risk Level Pair: " + dynamicRiskLevelPair.Key + " (" + dynamicRiskLevelPair.Value + ")", DebugType.Developer);

            return false;
        }*/

        /*[HarmonyPatch(nameof(LevelManager.AssignCalculatedRiskLevels))]
        [HarmonyPrefix]
        public static bool AssignCalculatedRiskLevelsPrefix()
        {
            Dictionary<int, string> assignmentRiskLevelDictionary = new Dictionary<int, string>();
            List<int> orderedCalculatedDifficultyList = new List<int>(LevelManager.dynamicRiskLevelDictionary.Values);
            orderedCalculatedDifficultyList.Sort();

            foreach (int calculatedDifficultyValue in orderedCalculatedDifficultyList)
            {
                Plugin.Logger.LogWarning("Checking:" + calculatedDifficultyValue);
                foreach (KeyValuePair<string, int> calculatedRiskLevel in LevelManager.dynamicRiskLevelDictionary)
                {
                    Plugin.Logger.LogInfo(calculatedRiskLevel.Key + "|" + calculatedRiskLevel.Value);
                    if (calculatedRiskLevel.Value == calculatedDifficultyValue)
                        assignmentRiskLevelDictionary.Add(calculatedDifficultyValue, calculatedRiskLevel.Key);
                }
            }

            //foreach (KeyValuePair<int, string> calculatedRiskLevel in assignmentRiskLevelDictionary)
            //    DebugHelper.Log("Ordered Calculated Risk Level: (" + calculatedRiskLevel.Value + ") - " + calculatedRiskLevel.Key, DebugType.Developer);

            foreach (ExtendedLevel customLevel in PatchedContent.CustomExtendedLevels)
            {
                if (customLevel.OverrideDynamicRiskLevelAssignment == false)
                {
                    int customLevelCalculatedDifficultyRating = customLevel.CalculatedDifficultyRating;
                    int closestCalculatedRiskLevelRating = orderedCalculatedDifficultyList[0];

                    closestCalculatedRiskLevelRating = orderedCalculatedDifficultyList.OrderBy(item => Math.Abs(customLevelCalculatedDifficultyRating - item)).First();

                    if (closestCalculatedRiskLevelRating != 0)
                        customLevel.SelectableLevel.riskLevel = assignmentRiskLevelDictionary[closestCalculatedRiskLevelRating];
                }
            }

            //List<ExtendedLevel> extendedLevelsOrdered = new List<ExtendedLevel>(PatchedContent.ExtendedLevels).OrderBy(o => o.CalculatedDifficultyRating).ToList();

            //foreach (ExtendedLevel extendedLevel in extendedLevelsOrdered)
            //    DebugHelper.Log(extendedLevel.NumberlessPlanetName + " (" + extendedLevel.SelectableLevel.riskLevel + ") " + " (" + extendedLevel.CalculatedDifficultyRating + ")", DebugType.Developer);
            return false;
        }*/

        /*public static int SetNearestAvailableRating(this Dictionary<int, string> dict, ExtendedLevel level, int rating)
        {
            int modifiedRating = rating;
            int finalRating = int.MinValue; //this value should NEVER be used.
            for(int i = 650; i < 670; i++)
            {
                if (dict.TryGetValue(i, out string value))
                    Plugin.Logger.LogWarning(value + ":" + i);
                else
                    Plugin.Logger.LogWarning("no value:" + i);
            }
            if(dict.ContainsValue(level.UniqueIdentificationName))
            {
                dict.Remove(dict.First(x => x.Value == level.UniqueIdentificationName).Key); //remove because it will be re-added within this method.
            }
            if (!dict.TryGetValue(modifiedRating, out string val1))
            {
                finalRating = modifiedRating; //actual rating is used
                Plugin.Logger.LogError(level.UniqueIdentificationName + " rating is unmodified: " + finalRating);
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
                        modifiedRating = Enumerable.Range(rating - 12, 25).Except(dict.Keys).FirstOrDefault();
                        if (!dict.TryGetValue(modifiedRating, out string val4))
                        {
                            finalRating = modifiedRating;
                        }
                    }
                }
                Plugin.Logger.LogError(level.UniqueIdentificationName + " rating is modified due to conflict with another moon: " + finalRating + "(original rating: " + rating + ").");

            }
            //int firstTry = Enumerable.Range(rating - 1, 3).Except(dict.Keys).FirstOrDefault();
            //Plugin.Logger.LogError(level.UniqueIdentificationName + " Jiggling1: " + firstTry);
            //int secondTry = Enumerable.Range(rating - 2, 5).Except(dict.Keys).FirstOrDefault();
            //Plugin.Logger.LogError(level.UniqueIdentificationName + " Jiggling2: " + secondTry);
            //int thirdTry = Enumerable.Range(rating - 10, 21).Except(dict.Keys).FirstOrDefault();
            //Plugin.Logger.LogError(level.UniqueIdentificationName + " Jiggling3: " + thirdTry);
            return finalRating;
        }*/

        /*private static int softlockAvoidance(ExtendedLevel level, int updatedRating)
        {
            int previousRating;
            int finalRating;
            ExtendedLevel patchedLevel = PatchedContent.CustomExtendedLevels.Find(x => x.UniqueIdentificationName == level.UniqueIdentificationName);
            //currentMoonRatingsList.RemoveAll(x => x.Value == level.SelectableLevel.PlanetName.ToString());
            int modifier = 0; //0
            if (LevelManager.dynamicRiskLevelDictionary.ContainsValue(updatedRating))
            {
                modifier = -1; //-1
                if (currentMoonRatingsList.Find(x => x.Key == (updatedRating + modifier)).Key != (updatedRating + modifier))
                {
                    modifier = -2; //-2
                    if (currentMoonRatingsList.Find(x => x.Key == (updatedRating + modifier)).Key != (updatedRating + modifier))
                    {
                        modifier = 1; //+1
                        if (currentMoonRatingsList.Find(x => x.Key == (updatedRating + modifier)).Key != (updatedRating + modifier))
                        {
                            modifier = 2; //+2
                            if (currentMoonRatingsList.Find(x => x.Key == (updatedRating + modifier)).Key != (updatedRating + modifier))
                            {
                                Plugin.Logger.LogError(level.SelectableLevel.PlanetName.ToString() + " could not reassign rating to a close rating, setting rating to a very negative number in the hope of avoiding a softlock. Please report this issue asap.");
                                for (int r = -6000; r < -4000; r++)
                                {
                                    if (currentMoonRatingsList.Find(x => x.Key == (updatedRating + r)).Key != (updatedRating + r))
                                    {
                                        modifier = r;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                Plugin.Logger.LogWarning("Duplicate Rating Found!: " + updatedRating.ToString() + " for moon: " + level.SelectableLevel.PlanetName.ToString() + " so modifying the rating to:" + (updatedRating + modifier));
                finalRating = updatedRating + modifier;
            }
            else
            {
                finalRating = updatedRating;
            }
            return finalRating;
        }

        private static int softlockAvoidanceOld(ExtendedLevel extendedLevel, int updatedRating) //always try to REDUCE the rating before trying to INCREASE it to fix this issue due to using an LLL dictionary
        {
            int finalRating;
            currentMoonRatingsList.RemoveAll(x => x.Value == extendedLevel.SelectableLevel.PlanetName.ToString());
            int modifier = 0; //0
            if (currentMoonRatingsList.Find(x => x.Key == updatedRating).Key == updatedRating)
            {
                modifier = -1; //-1
                if (currentMoonRatingsList.Find(x => x.Key == (updatedRating + modifier)).Key != (updatedRating + modifier))
                {
                    modifier = -2; //-2
                    if (currentMoonRatingsList.Find(x => x.Key == (updatedRating + modifier)).Key != (updatedRating + modifier))
                    {
                        modifier = 1; //+1
                        if (currentMoonRatingsList.Find(x => x.Key == (updatedRating + modifier)).Key != (updatedRating + modifier))
                        {
                            modifier = 2; //+2
                            if (currentMoonRatingsList.Find(x => x.Key == (updatedRating + modifier)).Key != (updatedRating + modifier))
                            {
                                Plugin.Logger.LogError(extendedLevel.SelectableLevel.PlanetName.ToString() + " could not reassign rating to a close rating, setting rating to a very negative number in the hope of avoiding a softlock. Please report this issue asap.");
                                for (int r = -6000; r < -4000; r++)
                                {
                                    if (currentMoonRatingsList.Find(x => x.Key == (updatedRating + r)).Key != (updatedRating + r))
                                    {
                                        modifier = r;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                Plugin.Logger.LogWarning("Duplicate Rating Found!: " + updatedRating.ToString() + " for moon: " + extendedLevel.SelectableLevel.PlanetName.ToString() + " so modifying the rating to:" + (updatedRating + modifier));
                finalRating = updatedRating + modifier;
            }
            else
            {
                finalRating = updatedRating;
            }
            return finalRating;
        }*/
    }
}