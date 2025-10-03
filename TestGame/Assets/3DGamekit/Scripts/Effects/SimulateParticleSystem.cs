using UnityEngine;

/// <summary>
/// Assures the particle systems are simulating once.
/// </summary>

namespace Gamekit3D
{
    public class SimulateParticleSystem : MonoBehaviour
    {
        public ParticleSystem[] systems;

        private void OnEnable()
        {
            for (var i = 0; i < systems.Length; i++)
                if (!systems[i].isPlaying)
                    systems[i].Simulate(0f);
        }
    }
}