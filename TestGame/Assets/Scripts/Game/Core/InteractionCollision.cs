using UnityEngine;
using UnityEngine.Events;

namespace Gamekit3D
{
    [RequireComponent(typeof(Collider))]
    public class InteractionCollision : MonoBehaviour
    {
        public LayerMask layers;
        public UnityEvent OnCollision;

        private void Reset()
        {
            layers = LayerMask.NameToLayer("Everything");
        }

        private void OnCollisionEnter(Collision c)
        {
            Debug.Log(c);
            if (0 != (layers.value & (1 << c.transform.gameObject.layer))) OnCollision.Invoke();
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawIcon(transform.position, "InteractionTrigger", false);
        }

        private void OnDrawGizmosSelected()
        {
            //need to inspect events and draw arrows to relevant gameObjects.
        }
    }
}