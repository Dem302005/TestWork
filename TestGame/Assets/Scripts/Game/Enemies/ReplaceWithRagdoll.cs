using UnityEngine;

namespace Gamekit3D
{
    public class ReplaceWithRagdoll : MonoBehaviour
    {
        public GameObject ragdollPrefab;

        public void Replace()
        {
            var ragdollInstance = Instantiate(ragdollPrefab, transform.position, transform.rotation);
            //need to disable it, otherwise when we copy over the hierarchy objects position/rotation, the ragdoll will try each time to 
            //"correct" the attached joint, leading to a deformed/glitched instance
            ragdollInstance.SetActive(false);

            var baseController = GetComponent<EnemyController>();

            var t = ragdollInstance.AddComponent<RigidbodyDelayedForce>();
            t.forceToAdd = baseController.externalForce;

            var ragdollCurrent = ragdollInstance.transform;
            var current = transform;
            var first = true;

            while (current != null && ragdollCurrent != null)
            {
                if (first || ragdollCurrent.name == current.name)
                {
                    //we only match part of the hierarchy that are named the same, except for the very first, as the 2 objects will have different name (but must have the same skeleton)
                    ragdollCurrent.rotation = current.rotation;
                    ragdollCurrent.position = current.position;
                    first = false;
                }

                if (current.childCount > 0)
                {
                    // Get first child.
                    current = current.GetChild(0);
                    ragdollCurrent = ragdollCurrent.GetChild(0);
                }
                else
                {
                    while (current != null)
                        if (current.parent == null || ragdollCurrent.parent == null)
                        {
                            // No more transforms to find.
                            current = null;
                            ragdollCurrent = null;
                        }
                        else if (current.GetSiblingIndex() == current.parent.childCount - 1 ||
                                 current.GetSiblingIndex() + 1 >= ragdollCurrent.parent.childCount)
                        {
                            // Need to go up one level.
                            current = current.parent;
                            ragdollCurrent = ragdollCurrent.parent;
                        }
                        else
                        {
                            // Found next sibling for next iteration.
                            current = current.parent.GetChild(current.GetSiblingIndex() + 1);
                            ragdollCurrent = ragdollCurrent.parent.GetChild(ragdollCurrent.GetSiblingIndex() + 1);
                            break;
                        }
                }
            }


            ragdollInstance.SetActive(true);
            Destroy(gameObject);
        }
    }
}