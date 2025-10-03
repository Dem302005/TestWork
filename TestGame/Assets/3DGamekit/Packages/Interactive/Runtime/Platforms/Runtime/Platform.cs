using UnityEngine;

namespace Gamekit3D.GameCommands
{
    public class Platform : MonoBehaviour
    {
        private const float k_SqrMaxCharacterMovement = 1f;
        public LayerMask layers;

        protected CharacterController m_CharacterController;

        private void OnTriggerExit(Collider other)
        {
            if (0 != (layers.value & (1 << other.gameObject.layer)))
                if (m_CharacterController != null && other.gameObject == m_CharacterController.gameObject)
                    m_CharacterController = null;
        }

        private void OnTriggerStay(Collider other)
        {
            if (0 != (layers.value & (1 << other.gameObject.layer)))
            {
                var character = other.GetComponent<CharacterController>();

                if (character != null)
                    m_CharacterController = character;
            }
        }

        public void MoveCharacterController(Vector3 deltaPosition)
        {
            if (m_CharacterController != null && deltaPosition.sqrMagnitude < k_SqrMaxCharacterMovement)
                m_CharacterController.Move(deltaPosition);
        }
    }
}