using System.Collections.Generic;

namespace Gamekit3D
{
    public static class EnemyCounter
    {
        private static readonly Dictionary<EnemyIdentifier.EnemyType, int> s_KilledEnemiesCount =
            new Dictionary<EnemyIdentifier.EnemyType, int>();

        public static void IncrementKillCount(EnemyIdentifier.EnemyType enemyType)
        {
            if (!s_KilledEnemiesCount.ContainsKey(enemyType)) s_KilledEnemiesCount[enemyType] = 0;
            s_KilledEnemiesCount[enemyType]++;
        }

        public static int GetKillCount(EnemyIdentifier.EnemyType enemyType)
        {
            if (s_KilledEnemiesCount.ContainsKey(enemyType)) return s_KilledEnemiesCount[enemyType];
            return 0;
        }

        public static void ResetCount()
        {
            s_KilledEnemiesCount.Clear();
        }
    }
}