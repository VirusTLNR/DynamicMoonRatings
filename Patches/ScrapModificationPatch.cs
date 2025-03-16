using HarmonyLib;
using LethalLevelLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Netcode;
using UnityEngine;

namespace DynamicMoonRatings.Patches
{
    internal class ScrapModificationPatch
    {
        [HarmonyPatch(typeof(RoundManager))]
        public class LevelManagerMoonRatingsPatch
        {
            [HarmonyPatch(nameof(RoundManager.SpawnScrapInLevel))]
            [HarmonyPrefix]
            public static bool ModifySpawnedScrapPrefix(RoundManager __instance)
            {
                if(Plugin.usageMode == "Risk Mode")
                {
                    Plugin.Logger.LogDebug("Mod is in RISK MODE, so scrap is not modified (nothing has been changed in SpawnScrapInLevel)");
                    return true;
                }
                Modules.ScrapGeneration.ScrapGenSelector(__instance, true);

                return true;
            }
        }
    }
}
