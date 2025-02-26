using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using LobbyCompatibility.Attributes;
using LobbyCompatibility.Enums;
using System.Collections.Generic;
using System.Reflection;

namespace DynamicMoonRatings
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("BMX.LobbyCompatibility", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("imabatby.lethallevelloader", BepInDependency.DependencyFlags.HardDependency)]
    [LobbyCompatibility(CompatibilityLevel.Everyone, VersionStrictness.None)]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance { get; private set; } = null!;
        internal new static ManualLogSource Logger { get; private set; } = null!;
        internal static Harmony? Harmony { get; set; }

        internal static int displayMode;
        internal static bool usageMode,speedrunningMode;

        private void Awake()
        {
            Logger = base.Logger;
            Instance = this;

            SetupConfig();
            RemoveOrphanedConfigs();
            Patch();

            Logger.LogInfo($"{PluginInfo.PLUGIN_GUID} v{PluginInfo.PLUGIN_VERSION} has loaded!");
        }

        internal static void Patch()
        {
            Harmony ??= new Harmony(PluginInfo.PLUGIN_GUID);

            Logger.LogDebug("Patching...");

            Harmony.PatchAll();

            Logger.LogDebug("Finished patching!");
        }

        internal static void Unpatch()
        {
            Logger.LogDebug("Unpatching...");

            Harmony?.UnpatchSelf();

            Logger.LogDebug("Finished unpatching!");
        }

        internal void SetupConfig()
        {
            usageMode = ((BaseUnityPlugin)this).Config.Bind<bool>("General", "Usage Mode", true, "TRUE = RISK MODE, FALSE = DIFFICULTY MODE --- risk mode adds scrap into the rating calculation - a lower rating means scrap is easier to get. difficulty mode removes scrap from the rating calculation and instead modifies the scrap for a moon based off the difficulty rating, higher rating = more scrap to be found.").Value;
            displayMode = ((BaseUnityPlugin)this).Config.Bind<int>("General", "Display Mode", 0, "0 = 'A###' (for example.. 'S256'), 1 = 'A' (for example 'D'), 2 = '####' (for example '4678').").Value;
            speedrunningMode = ((BaseUnityPlugin)this).Config.Bind<bool>("General", "Speedrunning Mode", false, "if you are competing to achieve a score on a leader board, turn on speed running mode to automatically display a watermark on your game (for recording) which denotes the moon rating, mod version, usage mode, display mode.").Value;
        }

        internal void RemoveOrphanedConfigs()
        {
            //for testing that orphan removel is working.
            //int orphan = ((BaseUnityPlugin)this).Config.Bind<int>("Test", "Orphan", 100, "just an orphan").Value;

            PropertyInfo orphanedEntriesProp = ((BaseUnityPlugin)this).Config.GetType().GetProperty("OrphanedEntries", BindingFlags.NonPublic | BindingFlags.Instance);

            var orphanedEntries = (Dictionary<BepInEx.Configuration.ConfigDefinition, string>)orphanedEntriesProp.GetValue(Config, null);
            if (orphanedEntries.Count != 0)
            {
                Plugin.Logger.LogInfo("Found Orphaned Config Entries - Removing them all as they are not needed anymore");
            }

            orphanedEntries.Clear(); // Clear orphaned entries (Unbinded/Abandoned entries)
            ((BaseUnityPlugin)this).Config.Save(); // Save the config file
        }
    }
}
