using System.Collections;
using UnityEngine;

namespace Gamekit3D.GameCommands
{
    [RequireComponent(typeof(Collider))]
    public class Teleporter : MonoBehaviour
    {
        public new Collider collider;
        public LayerMask layers;

        public GameObject enterEffect;
        public GameObject exitEffect;
        public Transform destinationTransform;
        public float delayTime;

        private WaitForSeconds delay;

        private void Awake()
        {
            delay = new WaitForSeconds(delayTime);
        }

        private void Reset()
        {
            collider = GetComponent<Collider>();
            collider.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (IsTeleportable(other))
                StartCoroutine(Activate(other.gameObject));
        }

        private IEnumerator Activate(GameObject teleportee)
        {
            if (destinationTransform)
            {
                foreach (var i in teleportee.GetComponentsInChildren<OnTeleportEvent>())
                    i.OnTeleport(this);
                if (enterEffect) enterEffect.SetActive(true);
                yield return delay;
                if (exitEffect) exitEffect.SetActive(true);
                teleportee.transform.position = destinationTransform.position;
                teleportee.transform.rotation = destinationTransform.rotation;
            }
        }

        private bool IsTeleportable(Collider other)
        {
            return 0 != (layers.value & (1 << other.gameObject.layer));
        }
    }
}