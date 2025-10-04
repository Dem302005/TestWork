using UnityEngine;

namespace Gamekit3D
{
    public class SkyboxLookAt : MonoBehaviour
    {
        public Transform target;

        private void Update()
        {
            transform.LookAt(target);
        }
    }
}