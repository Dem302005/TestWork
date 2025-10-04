using UnityEngine;

namespace Gamekit3D.GameCommands
{
    public class SendOnCollisionExit : SendGameCommand
    {
        public LayerMask layers;

        private void OnCollisionExit(Collision collision)
        {
            if (0 != (layers.value & (1 << collision.gameObject.layer))) Send();
        }
    }
}