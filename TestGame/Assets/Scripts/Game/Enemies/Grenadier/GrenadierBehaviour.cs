using UnityEngine;
using UnityEngine.AI;

namespace Gamekit3D
{
    [RequireComponent(typeof(EnemyController))]
    [RequireComponent(typeof(NavMeshAgent))]
    public class GrenadierBehaviour : MonoBehaviour
    {
        public enum OrientationState
        {
            IN_TRANSITION,
            ORIENTED_ABOVE,
            ORIENTED_FACE
        }

        public static readonly int hashInPursuitParam = Animator.StringToHash("InPursuit");
        public static readonly int hashSpeedParam = Animator.StringToHash("Speed");
        public static readonly int hashTurnAngleParam = Animator.StringToHash("Angle");
        public static readonly int hashTurnTriggerParam = Animator.StringToHash("TurnTrigger");
        public static readonly int hashMeleeAttack = Animator.StringToHash("MeleeAttack");
        public static readonly int hashRangeAttack = Animator.StringToHash("RangeAttack");
        public static readonly int hashHitParam = Animator.StringToHash("Hit");
        public static readonly int hashDeathParam = Animator.StringToHash("Death");
        public static readonly int hashRotateAttackParam = Animator.StringToHash("Rotate");

        public static readonly int hashIdleState = Animator.StringToHash("GrenadierIdle");

        public TargetScanner playerScanner;

        public float meleeRange = 4.0f;
        public float rangeRange = 10.0f;

        public MeleeWeapon fistWeapon;
        public RangeWeapon grenadeLauncher;

        public GameObject shield;

        public SkinnedMeshRenderer coreRenderer;

        [Header("Audio")] public RandomAudioPlayer deathAudioPlayer;

        public RandomAudioPlayer damageAudioPlayer;
        public RandomAudioPlayer footstepAudioPlayer;
        public RandomAudioPlayer throwAudioPlayer;
        public RandomAudioPlayer punchAudioPlayer;
        protected Material m_CoreMaterial;

        protected Damageable m_Damageable;

        protected EnemyController m_EnemyController;

        //used to store the position of the target when the Grenadier decide to shoot, so if the player
        //move between the start of the animation and the actual grenade launch, it shoot were it was not where it is now
        protected Vector3 m_GrenadeTarget;
        protected NavMeshAgent m_NavMeshAgent;
        protected Color m_OriginalCoreMaterial;

        protected float m_ShieldActivationTime;

        protected PlayerController m_Target;

        public EnemyController controller => m_EnemyController;

        public bool shieldUp => shield.activeSelf;

        public PlayerController target => m_Target;
        public Damageable damageable => m_Damageable;

        private void Update()
        {
            if (m_ShieldActivationTime > 0)
            {
                m_ShieldActivationTime -= Time.deltaTime;

                if (m_ShieldActivationTime <= 0.0f)
                    DeactivateShield();
            }
        }


        private void OnEnable()
        {
            m_EnemyController = GetComponent<EnemyController>();
            m_NavMeshAgent = GetComponent<NavMeshAgent>();

            SceneLinkedSMB<GrenadierBehaviour>.Initialise(m_EnemyController.animator, this);

            fistWeapon.SetOwner(gameObject);
            fistWeapon.EndAttack();

            m_CoreMaterial = coreRenderer.materials[1];
            m_OriginalCoreMaterial = m_CoreMaterial.GetColor("_Color2");

            m_EnemyController.animator.Play(hashIdleState, 0, Random.value);

            shield.SetActive(false);

            m_Damageable = GetComponentInChildren<Damageable>();
        }

#if UNITY_EDITOR

        private void OnDrawGizmosSelected()
        {
            playerScanner.EditorGizmo(transform);
        }

#endif

        public void FindTarget()
        {
            m_Target = playerScanner.Detect(transform);
        }

        public void StartPursuit()
        {
            m_EnemyController.animator.SetBool(hashInPursuitParam, true);
        }

        public void StopPursuit()
        {
            m_EnemyController.animator.SetBool(hashInPursuitParam, false);
        }

        public void StartAttack()
        {
            fistWeapon.BeginAttack(true);
        }

        public void EndAttack()
        {
            fistWeapon.EndAttack();
        }

        public void Hit()
        {
            damageAudioPlayer.PlayRandomClip();
            m_EnemyController.animator.SetTrigger(hashHitParam);
            m_CoreMaterial.SetColor("_Color2", Color.red);
        }

        public void Die()
        {
            deathAudioPlayer.PlayRandomClip();
            m_EnemyController.animator.SetTrigger(hashDeathParam);
        }

        public void ActivateShield()
        {
            shield.SetActive(true);
            m_ShieldActivationTime = 3.0f;
            m_Damageable.SetColliderState(false);
        }

        public void DeactivateShield()
        {
            shield.SetActive(false);
            m_Damageable.SetColliderState(true);
        }

        public void ReturnVulnerable()
        {
            m_CoreMaterial.SetColor("_Color2", m_OriginalCoreMaterial);
        }

        public void RememberTargetPosition()
        {
            m_GrenadeTarget = m_Target.transform.position;
        }

        public void PlayStep()
        {
            footstepAudioPlayer.PlayRandomClip();
        }

        public void Shoot()
        {
            throwAudioPlayer.PlayRandomClip();

            var toTarget = m_GrenadeTarget - transform.position;

            //the grenade is launched a couple of meters in "front" of the player, because it bounce and roll, to make it a bit ahrder for the player
            //to avoid it
            var target = transform.position + (toTarget - toTarget * 0.3f);

            grenadeLauncher.Attack(target);
        }

        public OrientationState OrientTowardTarget()
        {
            var v = m_Target.transform.position - transform.position;
            var above = v.y > 0.3f;
            v.y = 0;

            var angle = Vector3.SignedAngle(transform.forward, v, Vector3.up);

            if (Mathf.Abs(angle) < 20.0f)
            {
                //for a very small angle, we directly rotate the model
                transform.forward = v.normalized;
                // if the player was above the player we return false to tell the Idle state 
                // that we want a "shield up" attack as our punch attack wouldn't reach it.
                return above ? OrientationState.ORIENTED_ABOVE : OrientationState.ORIENTED_FACE;
            }

            m_EnemyController.animator.SetFloat(hashTurnAngleParam, angle / 180.0f);
            m_EnemyController.animator.SetTrigger(hashTurnTriggerParam);
            return OrientationState.IN_TRANSITION;
        }
    }
}