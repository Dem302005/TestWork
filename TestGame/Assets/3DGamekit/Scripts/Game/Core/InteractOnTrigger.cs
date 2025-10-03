using UnityEngine;
using UnityEngine.Events;

namespace Gamekit3D
{
    [RequireComponent(typeof(Collider))]
    public class InteractOnTrigger : MonoBehaviour
    {
        public LayerMask layers;
        public UnityEvent OnEnter, OnExit;
        public InventoryController.InventoryChecker[] inventoryChecks;
        private new Collider collider;

        private void Reset()
        {
            layers = LayerMask.NameToLayer("Everything");
            collider = GetComponent<Collider>();
            collider.isTrigger = true;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawIcon(transform.position, "InteractionTrigger", false);
        }

        private void OnDrawGizmosSelected()
        {
            //need to inspect events and draw arrows to relevant gameObjects.
        }

        private void OnTriggerEnter(Collider other)
        {
            if (0 != (layers.value & (1 << other.gameObject.layer))) ExecuteOnEnter(other);
        }

        private void OnTriggerExit(Collider other)
        {
            if (0 != (layers.value & (1 << other.gameObject.layer))) ExecuteOnExit(other);
        }

        protected virtual void ExecuteOnEnter(Collider other)
        {
            OnEnter.Invoke();
            for (var i = 0; i < inventoryChecks.Length; i++)
                inventoryChecks[i].CheckInventory(other.GetComponentInChildren<InventoryController>());
        }

        protected virtual void ExecuteOnExit(Collider other)
        {
            OnExit.Invoke();
        }
    }
}