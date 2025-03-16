using LethalLevelLoader;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static LethalLib.Modules.Enemies;

namespace DynamicMoonRatings.Modules.CalculationsV1
{
    internal class Enemies
    {
        internal static float EnemiesCalc(ExtendedLevel level)
        {
            SelectableLevel sL = level.SelectableLevel;
            int maxDaytimeEnemyPowerCount = sL.maxDaytimeEnemyPowerCount; //outside daytime?
            int maxOutsideEnemyPowerCount = sL.maxOutsideEnemyPowerCount; //outside nighttime?
            int maxEnemyPowerCount = sL.maxEnemyPowerCount; //inside enemies?
            var dayTimeEnemies = sL.DaytimeEnemies; //outside daytime?
            var outsideEnemies = sL.OutsideEnemies; //outside nighttime?
            var enemies = sL.Enemies; //inside enemies?
            //float enemiesModifier = checkEnemiesForAddedDifficulty(dayTimeEnemies, outsideEnemies, enemies);
            //float enemiesModifier = (checkEnemies(level) * 3) + ((maxDaytimeEnemyPowerCount + maxEnemyPowerCount + maxOutsideEnemyPowerCount) * 30);
            float enemiesModifier = (checkEnemies(level) * 3) + ((checkAveragePower(dayTimeEnemies, maxDaytimeEnemyPowerCount) + checkAveragePower(outsideEnemies, maxOutsideEnemyPowerCount) + checkAveragePower(enemies, maxEnemyPowerCount)));
            return enemiesModifier;
        }

        private static float checkEnemiesForAddedDifficulty(List<SpawnableEnemyWithRarity> day, List<SpawnableEnemyWithRarity> night, List<SpawnableEnemyWithRarity> inside)
        {
            List<SpawnableEnemyWithRarity> fullList = new List<SpawnableEnemyWithRarity>();
            fullList.AddRange(day);
            fullList.AddRange(night);
            fullList.AddRange(inside);
            float enemyDifficultyModifier = 1f;
            foreach (SpawnableEnemyWithRarity enemy in fullList)
            {
                Plugin.Logger.LogDebug("Enemy = " + enemy.enemyType.name);
                /*string n = enemy.enemyType.enemyName;
                switch (n)
                {
                    case "SpringMan":
                    case "ClaySurgeon":
                        enemyDifficultyModifier += (1.50f * enemy.rarity);
                        break;
                    case "Nutcracker":
                    case "RadMech":
                    case "DressGirl":
                    case "CaveDweller":
                    case "Jester":
                        enemyDifficultyModifier += (1.25f * enemy.rarity);
                        break;
                    case "MaskedPlayerEnemy":
                    case "Flowerman":
                    case "MouthDog":
                    case "ForestGiant":
                    case "SandWorm":
                    case "Crawler":
                    case "SandSpider":
                    case "Butler":
                        enemyDifficultyModifier += (1.10f * enemy.rarity);
                        break;
                    default:
                        enemyDifficultyModifier += (1f * enemy.rarity);
                        break;
                }*/
                enemyDifficultyModifier += ((enemy.enemyType.PowerLevel * 100) / (enemy.rarity / 10));
            }
            return (enemyDifficultyModifier);
        }

        private static float checkAveragePower(List<SpawnableEnemyWithRarity> list, int maxPower)
        {
            int counter = 0;
            float st = 0;
            float total = 0;

            foreach (SpawnableEnemyWithRarity enemy in list)
            {
                float ep = enemy.enemyType.PowerLevel;
                float er = enemy.rarity;
                float ev = ep * er;
                counter++;
                st += ev;
                total = (st / counter) * maxPower;
                //Plugin.Logger.LogWarning(enemy.enemyType.enemyName + "|" + ep.ToString() + "|" + er.ToString() + "|" + ev.ToString());
            }
            //Plugin.Logger.LogWarning("Total = " + total);
            return total;
        }

        private static float checkEnemies(ExtendedLevel level)
        {
            float enemiesData = 0;
            float enemyValue = 0;
            foreach (SpawnableEnemyWithRarity spawnableEnemy in level.SelectableLevel.Enemies.Concat(level.SelectableLevel.OutsideEnemies).Concat(level.SelectableLevel.DaytimeEnemies))
            {
                if (spawnableEnemy.rarity != 0 && spawnableEnemy.enemyType != null)
                    if ((spawnableEnemy.rarity / 10) != 0)
                        enemyValue += (spawnableEnemy.enemyType.PowerLevel * 100) / (spawnableEnemy.rarity / 10);
            }
            enemiesData += Mathf.RoundToInt(enemyValue);

            return enemiesData;
        }
    }
}
