using UnityEngine;
using UnityEngine.AI;

namespace Gamekit3D
{
    [DefaultExecutionOrder(-1)]
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyController : MonoBehaviour
    {
        private const float k_GroundedRayDistance = .8f;
        public bool interpolateTurning;
        public bool applyAnimationRotation;
        protected Animator m_Animator;
        protected Vector3 m_ExternalForce;
        protected bool m_ExternalForceAddGravity = true;
        protected bool m_FollowNavmeshAgent;
        protected bool m_Grounded;

        protected NavMeshAgent m_NavMeshAgent;

        protected Rigidbody m_Rigidbody;
        protected bool m_UnderExternalForce;

        public Animator animator => m_Animator;
        public Vector3 externalForce => m_ExternalForce;
        public NavMeshAgent navmeshAgent => m_NavMeshAgent;
        public bool followNavmeshAgent => m_FollowNavmeshAgent;
        public bool grounded => m_Grounded;

        private void FixedUpdate()
        {
            animator.speed = PlayerInput.Instance != null && PlayerInput.Instance.HaveControl() ? 1.0f : 0.0f;

            CheckGrounded();

            if (m_UnderExternalForce)
                ForceMovement();
        }

        private void OnEnable()
        {
            m_NavMeshAgent = GetComponent<NavMeshAgent>();
            m_Animator = GetComponent<Animator>();
            m_Animator.updateMode = AnimatorUpdateMode.AnimatePhysics;

            m_NavMeshAgent.updatePosition = false;

            m_Rigidbody = GetComponentInChildren<Rigidbody>();
            if (m_Rigidbody == null)
                m_Rigidbody = gameObject.AddComponent<Rigidbody>();

            m_Rigidbody.isKinematic = true;
            m_Rigidbody.useGravity = false;
            m_Rigidbody.interpolation = RigidbodyInterpolation.Interpolate;

            m_FollowNavmeshAgent = true;
        }

        private void OnAnimatorMove()
        {
            if (m_UnderExternalForce)
                return;

            if (m_FollowNavmeshAgent)
            {
                m_NavMeshAgent.speed = (m_Animator.deltaPosition / Time.deltaTime).magnitude;
                transform.position = m_NavMeshAgent.nextPosition;
            }
            else
            {
                RaycastHit hit;
                if (!m_Rigidbody.SweepTest(m_Animator.deltaPosition.normalized, out hit,
                        m_Animator.deltaPosition.sqrMagnitude))
                    m_Rigidbody.MovePosition(m_Rigidbody.position + m_Animator.deltaPosition);
            }

            if (applyAnimationRotation) transform.forward = m_Animator.deltaRotation * transform.forward;
        }

        private void CheckGrounded()
        {
            RaycastHit hit;
            var ray = new Ray(transform.position + Vector3.up * k_GroundedRayDistance * 0.5f, -Vector3.up);
            m_Grounded = Physics.Raycast(ray, out hit, k_GroundedRayDistance, Physics.AllLayers,
                QueryTriggerInteraction.Ignore);
        }

        private void ForceMovement()
        {
            if (m_ExternalForceAddGravity)
                m_ExternalForce += Physics.gravity * Time.deltaTime;

            RaycastHit hit;
            var movement = m_ExternalForce * Time.deltaTime;
            if (!m_Rigidbody.SweepTest(movement.normalized, out hit, movement.sqrMagnitude))
                m_Rigidbody.MovePosition(m_Rigidbody.position + movement);

            m_NavMeshAgent.Warp(m_Rigidbody.position);
        }

        public void SetFollowNavmeshAgent(bool follow)
        {
            if (!follow && m_NavMeshAgent.enabled)
                m_NavMeshAgent.ResetPath();
            else if (follow && !m_NavMeshAgent.enabled) m_NavMeshAgent.Warp(transform.position);

            m_FollowNavmeshAgent = follow;
            m_NavMeshAgent.enabled = follow;
        }

        public void AddForce(Vector3 force, bool useGravity = true)
        {
            if (m_NavMeshAgent.enabled)
                m_NavMeshAgent.ResetPath();

            m_ExternalForce = force;
            m_NavMeshAgent.enabled = false;
            m_UnderExternalForce = true;
            m_ExternalForceAddGravity = useGravity;
        }

        public void ClearForce()
        {
            m_UnderExternalForce = false;
            m_NavMeshAgent.enabled = true;
        }

        public void SetForward(Vector3 forward)
        {
            var targetRotation = Quaternion.LookRotation(forward);

            if (interpolateTurning)
                targetRotation = Quaternion.RotateTowards(transform.rotation, targetRotation,
                    m_NavMeshAgent.angularSpeed * Time.deltaTime);

            transform.rotation = targetRotation;
        }

        public bool SetTarget(Vector3 position)
        {
            return m_NavMeshAgent.SetDestination(position);
        }
    }
}