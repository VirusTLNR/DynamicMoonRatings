using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;

namespace DynamicMoonRatings.Patches
{
    [HarmonyPatch(typeof(GameNetworkManager))]
    public class GameNetworkManagerPatch
    {
        [HarmonyPatch(nameof(GameNetworkManager.OnApplicationQuit))]
        [HarmonyPrefix]
        public static bool GameNetworkManagerOnApplicationQuitRemoveOrphans_Prefix()
        {
            Plugin.Logger.LogDebug("Checking for Orphans OnApplicationQuit");
            Plugin.Instance.RemoveOrphanedConfigs();
            return true;
        }
    }
}
