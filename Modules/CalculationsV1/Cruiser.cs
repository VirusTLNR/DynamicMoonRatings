using LethalLevelLoader;

namespace DynamicMoonRatings.Modules.CalculationsV1
{
    internal class Cruiser
    {
        internal static float CruiserCalc(ExtendedLevel level)
        {
            float cruiserPriceInShop = LethalLevelLoader.PatchedContent.ExtendedBuyableVehicles[0].BuyableVehicle.creditsWorth;
            //foreach(var car in LethalLevelLoader.PatchedContent.ExtendedBuyableVehicles)
            //{
            //    Plugin.Logger.LogError("Name=" + car.name);
            //    Plugin.Logger.LogWarning("DisplayName=" + car.BuyableVehicle.vehicleDisplayName);
            //    Plugin.Logger.LogWarning("CreditsWorth=" + car.BuyableVehicle.creditsWorth);
            //}
            //Plugin.Logger.LogError("CruiserCreditsWorth = " + cruiserPriceInShop);
            float mentionsCruiserModifier;
            if (level.SelectableLevel.LevelDescription.Contains("Cruiser")) //is surely a cruiser moon so "add" difficulty points...
            {
                mentionsCruiserModifier = cruiserPriceInShop;
            }
            else
            {
                mentionsCruiserModifier = 0;
            }
            return mentionsCruiserModifier;
        }
    }
}
