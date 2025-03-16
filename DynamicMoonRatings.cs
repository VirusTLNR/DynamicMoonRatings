using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using LobbyCompatibility.Attributes;
using LobbyCompatibility.Configuration;
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

        internal static string displayMode, usageMode, ratingsVersion, scrapGenVersion;
        internal static int difficultyModeMinScrapMod,difficultyModeMaxScrapMod;
        internal static bool speedrunningMode;

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
            //general
            usageMode = ((BaseUnityPlugin)this).Config.Bind<string>("General", "Usage Mode", "Risk Mode", new ConfigDescription(
                                                                                                "Risk Mode - adds scrap into the rating calculation - a lower rating means scrap is easier to get. " +
                                                                                                " \u000aDifficulty Mode - removes scrap from the rating calculation and instead modifies the scrap for a moon based off the difficulty rating, higher rating = more scrap to be found.",
                                                                                                new AcceptableValueList<string>(new string[] { "Risk Mode", "Difficulty Mode"}))).Value;
            ratingsVersion = ((BaseUnityPlugin)this).Config.Bind<string>("General", "Rating Version", "V1", new ConfigDescription(
                                                                                                "The version of the ratings calculation, as I update the mod, I may update this calculation, if so I will add more versions to this setting so you can stick with an old calculation you like, or try a new one, details will be in the readme.",
                                                                                                new AcceptableValueList<string>(new string[] { "CUSTOM","LLL","V1" }))).Value;
            scrapGenVersion = ((BaseUnityPlugin)this).Config.Bind<string>("General", "ScrapGen  Version", "V1", new ConfigDescription(
                                                                                    "[Difficulty Mode Only!] The version of the scrap generation calculation in Difficulty Mode, as I update the mod, I may update this calculation, if so I will add more versions to this setting so you can stick with an old calculation you like, or try a new one, details will be in the readme.",
                                                                                    new AcceptableValueList<string>(new string[] { "V1" }))).Value;
            difficultyModeMinScrapMod = ((BaseUnityPlugin)this).Config.Bind<int>("General", "Base Min Scrap Value", 300, new ConfigDescription("[Difficulty Mode Only!] The max scrap value for a 0 rated moon in Difficulty Mode, if you use Risk Mode, this will have no effect", new AcceptableValueRange<int>(50, 1500))).Value;
            difficultyModeMaxScrapMod = ((BaseUnityPlugin)this).Config.Bind<int>("General", "Base Max Scrap Value", 600, new ConfigDescription("[Difficulty Mode Only!] The min scrap value for a 0 rated moon in Difficulty Mode, if you use Risk Mode, this will have no effect", new AcceptableValueRange<int>(100, 3000))).Value;
            speedrunningMode = ((BaseUnityPlugin)this).Config.Bind<bool>("UI", "Speedrunning Mode", false, "(does nothing currently) if you are competing to achieve a score on a leader board, turn on speed running mode to automatically display a watermark on your game (for recording) which denotes the moon rating, mod version, usage mode, display mode. If you have 'A' display mode selected, it will switch you to 'A###' display mode instead.").Value;
            
            //display
            displayMode = ((BaseUnityPlugin)this).Config.Bind<string>("UI", "Display Mode", "A###", new ConfigDescription(
                                                                                                "Displayed Format in the moons list.. " +
                                                                                                " \u000a'A###' = like 'S256'" +
                                                                                                " \u000a'A' = like 'D'" +
                                                                                                " \u000a'####' = like '4678'.",
                                                                                                new AcceptableValueList<string>(new string[] { "A###", "A", "####" }))).Value;
            
            //custom moon ratings


            //custom tiers

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
