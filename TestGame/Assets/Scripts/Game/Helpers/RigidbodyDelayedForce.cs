using UnityEngine;

namespace Gamekit3D
{
    public class RigidbodyDelayedForce : MonoBehaviour
    {
        public Vector3 forceToAdd;

        private void Start()
        {
            var rigidbodies = gameObject.GetComponentsInChildren<Rigidbody>();

            for (var i = 0; i < rigidbodies.Length; ++i)
            {
                rigidbodies[i].maxAngularVelocity = 45;
                rigidbodies[i].angularVelocity = transform.right * -45.0f;
                rigidbodies[i].velocity = forceToAdd;
            }
        }
    }
}