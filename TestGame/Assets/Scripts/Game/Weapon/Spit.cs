using UnityEngine;

namespace Gamekit3D
{
    public class Spit : GrenadierGrenade
    {
        protected override void OnCollisionEnter(Collision other)
        {
            base.OnCollisionEnter(other);

            if (explosionTimer < 0)
                Explosion();
        }
    }
}