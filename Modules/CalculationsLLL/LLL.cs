using LethalLevelLoader;
using System.Linq;
using UnityEngine;
using static UnityEngine.Rendering.HighDefinition.ScalableSettingLevelParameter;

namespace DynamicMoonRatings.Modules.CalculationsLLL
{
    internal class LLL
    {
        internal static int CalculateSameAsLLL(ExtendedLevel extendedLevel)
        {
            int num = 0;
            string text = "Calculated Difficulty Rating For ExtendedLevel: " + extendedLevel.NumberlessPlanetName + "(" + extendedLevel.SelectableLevel.riskLevel + ") ----- ";
            int routePrice = extendedLevel.RoutePrice;
            routePrice += extendedLevel.SelectableLevel.maxTotalScrapValue;
            num += routePrice;
            text = text + "Baseline Route Value: " + routePrice + ", ";
            int num2 = 0;
            foreach (SpawnableItemWithRarity item in extendedLevel.SelectableLevel.spawnableScrap)
            {
                if (item.spawnableItem != null && (item.spawnableItem.minValue + item.spawnableItem.maxValue) * 5 != 0 && item.rarity != 0 && item.rarity / 10 != 0)
                {
                    num2 += (item.spawnableItem.maxValue - item.spawnableItem.minValue) / (item.rarity / 10);
                }
            }

            num += num2;
            text = text + "Scrap Value: " + num2 + ", ";
            int num3 = (extendedLevel.SelectableLevel.maxEnemyPowerCount + extendedLevel.SelectableLevel.maxOutsideEnemyPowerCount + extendedLevel.SelectableLevel.maxDaytimeEnemyPowerCount) * 15;
            num3 *= 2;
            num += num3;
            text = text + "Enemy Spawn Value: " + num3 + ", ";
            float num4 = 0f;
            foreach (SpawnableEnemyWithRarity item2 in extendedLevel.SelectableLevel.Enemies.Concat(extendedLevel.SelectableLevel.OutsideEnemies).Concat(extendedLevel.SelectableLevel.DaytimeEnemies))
            {
                if (item2.rarity != 0 && item2.enemyType != null && item2.rarity / 10 != 0)
                {
                    num4 += item2.enemyType.PowerLevel * 100f / (float)(item2.rarity / 10);
                }
            }

            num += Mathf.RoundToInt(num4);
            text = text + "Enemy Value: " + num4 + ", ";
            text = text + "Calculated Difficulty Value: " + num + ", ";
            num += Mathf.RoundToInt((float)num * (extendedLevel.SelectableLevel.factorySizeMultiplier * 0.5f));
            text = text + "Factory Size Multiplier: " + extendedLevel.SelectableLevel.factorySizeMultiplier + ", ";
            text = text + "Multiplied Calculated Difficulty Value: " + num;
            return num;
        }
    }
}
