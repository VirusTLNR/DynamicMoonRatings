using BepInEx.Configuration;
using HarmonyLib;
using LethalLevelLoader.Tools;

namespace DynamicMoonRatings.Patches
{
    [HarmonyPatch(typeof(StartOfRound))]
    public class StartOfRoundPatch
    {
        [HarmonyPatch(nameof(StartOfRound.Awake))]
        [HarmonyPostfix]
        public static void StartOfRoundAwakeLateConfigBinding_Postfix()
        {
            Plugin.Instance.SetupLateConfig();
        }
    }
}
