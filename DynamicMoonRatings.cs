using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using DynamicMoonRatings.Modules;
using HarmonyLib;
using LethalLevelLoader;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace DynamicMoonRatings
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("BMX.LobbyCompatibility", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("imabatby.lethallevelloader", BepInDependency.DependencyFlags.HardDependency)]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance { get; private set; } = null!;
        internal new static ManualLogSource Logger { get; private set; } = null!;
        internal static Harmony? Harmony { get; set; }

        internal static string displayMode, usageMode, ratingsVersion, scrapGenVersion;
        internal static int difficultyModeMinScrapMod,difficultyModeMaxScrapMod, tierCount;
        internal static bool speedrunningMode;
        internal static List<Modules.CustomTier> customTiers = new List<Modules.CustomTier>();
        internal static List<Modules.CustomRating> customRatings = new List<Modules.CustomRating>();
        internal static bool moonConfigLoaded = false; //check for if the moon config is loaded, so you dont try to update moon ratings until AFTER the config has been loaded when using custom ratings

        private void Awake()
        {
            Logger = base.Logger;
            Instance = this;

            if (LobbyCompatibilityChecker.Enabled)
            {
                Logger.LogInfo($"BMX.LobbyCompatibility has been found, Initiating Soft Dependency!");
                LobbyCompatibilityChecker.Init();
            }

            SetupConfig();
            //RemoveOrphanedConfigs(); //cant do this here anymore, so now doing it on CLOSE
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

            //display
            speedrunningMode = ((BaseUnityPlugin)this).Config.Bind<bool>("Display", "Speedrunning Mode", false, "(does nothing currently) if you are competing to achieve a score on a leader board, turn on speed running mode to automatically display a watermark on your game (for recording) which denotes the moon rating, mod version, usage mode, display mode. If you have 'A' display mode selected, it will switch you to 'A###' display mode instead.").Value;


            displayMode = ((BaseUnityPlugin)this).Config.Bind<string>("Display", "Display Mode", "A###", new ConfigDescription(
                                                                                                "Displayed Format in the moons list.. " +
                                                                                                " \u000a'A###' = like 'S256'" +
                                                                                                " \u000a'A' = like 'D'" +
                                                                                                " \u000a'####' = like '4678'.",
                                                                                                new AcceptableValueList<string>(new string[] { "A###", "A", "####" }))).Value;
            
            //custom tiers - must be before custom moons in the loading process as custom moons uses the first tiers rating as a default.
            tierCount = ((BaseUnityPlugin)this).Config.Bind<int>("TierCustomisation", "Tier Count", 10, new ConfigDescription("How many tiers you would like.. after changing this value you will need to start the game and close it to modify the other values below (WARNING - changing to a lower value will result in lost ratings/displayed names for higher tiers you no longer use!)", new AcceptableValueRange<int>(1, 25))).Value;
            customTiers.Add(new CustomTier() { uniqueName = "Tier 0", minRating = Int16.MinValue, displayName = "Unknown" }); //for any moon without a config
            for (int t = 1; t <= tierCount; t++) //allow for 25 custom tiers
            {
                Modules.CustomTier ct = new Modules.CustomTier();
                string name = "Tier" + t.ToString("00");
                int rating = ((BaseUnityPlugin)this).Config.Bind<int>("TierCustomisation", name + " Min Rating Threshold", ((t-1) * 1000) , "The Rating for " + name).Value;
                string display = ((BaseUnityPlugin)this).Config.Bind<string>("TierCustomisation", name + " Display Name", "T"+t.ToString("00"), "The Displayed String for " + name + ". Only first 4 chars are readable on the moon catalog").Value;
                ct.uniqueName = name;
                ct.minRating = rating;
                ct.displayName = display;
                customTiers.Add(ct);
            }
        }

        internal void SetupLateConfig()
        {
            Plugin.Logger.LogDebug("Moons to add to config = " + Modules.CustomModes.moons.Count);
            foreach (ExtendedLevel level in Modules.CustomModes.moons)
            {
                Modules.CustomRating cr = new Modules.CustomRating();
                Plugin.Logger.LogDebug("Adding Moon to config:- " + level.UniqueIdentificationName + " as: " + level.NumberlessPlanetName);
                string name = level.UniqueIdentificationName;
                string display = level.NumberlessPlanetName;
                //int rating = ((BaseUnityPlugin)this).Config.Bind<int>("CustomRatings", name + " Custom Rating", int.MinValue, "The Rating for " + name + " as set by you to fit in with your custom tiers.").Value;
                string rating = ((BaseUnityPlugin)this).Config.Bind<string>("RatingsCustomisation", display, null, "The Rating for " + display + " as set by you to fit in with your custom tiers. Any value that is not an integer will be rejected and set as 'Unrated' tier").Value;
                cr.uniqueName = name;
                if (int.TryParse(rating, out int resultRating))
                {
                    cr.rating = resultRating;
                }
                else
                {
                    cr.rating = Int16.MinValue;
                }
                cr.displayName = display;
                customRatings.Add(cr);
            }
            moonConfigLoaded = true; //set this to true so that the moon ratings can be updated after the config has been loaded.
            foreach (ExtendedLevel level in Modules.CustomModes.moons)
            {
                LevelManager.CalculateExtendedLevelDifficultyRating(level);
            }
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
