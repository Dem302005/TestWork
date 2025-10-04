using UnityEngine;

namespace Gamekit3D
{
    public class EnemyIdentifier : MonoBehaviour
    {
        public enum EnemyType
        {
            Grenadier,
            Chomper,
            Spitter
        }

        public EnemyType type;
    }
}