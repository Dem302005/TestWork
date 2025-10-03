using System;
using Gamekit3D.Message;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gamekit3D
{
    [DefaultExecutionOrder(100)]
    public class ChomperBehavior : MonoBehaviour, IMessageReceiver
    {
        public static readonly int hashInPursuit = Animator.StringToHash("InPursuit");
        public static readonly int hashAttack = Animator.StringToHash("Attack");
        public static readonly int hashHit = Animator.StringToHash("Hit");
        public static readonly int hashVerticalDot = Animator.StringToHash("VerticalHitDot");
        public static readonly int hashHorizontalDot = Animator.StringToHash("HorizontalHitDot");
        public static readonly int hashThrown = Animator.StringToHash("Thrown");
        public static readonly int hashGrounded = Animator.StringToHash("Grounded");
        public static readonly int hashVerticalVelocity = Animator.StringToHash("VerticalVelocity");
        public static readonly int hashSpotted = Animator.StringToHash("Spotted");
        public static readonly int hashNearBase = Animator.StringToHash("NearBase");

        public static readonly int hashIdleState = Animator.StringToHash("ChomperIdle");

        public MeleeWeapon meleeWeapon;
        public TargetScanner playerScanner;

        [Tooltip("Time in seconde before the Chomper stop pursuing the player when the player is out of sight")]
        public float timeToStopPursuit;

        [Header("Audio")] public RandomAudioPlayer attackAudio;

        public RandomAudioPlayer frontStepAudio;
        public RandomAudioPlayer backStepAudio;
        public RandomAudioPlayer hitAudio;
        public RandomAudioPlayer gruntAudio;
        public RandomAudioPlayer deathAudio;
        public RandomAudioPlayer spottedAudio;

        [NonSerialized] public float attackDistance = 3;

        protected EnemyController m_Controller;
        protected TargetDistributor.TargetFollower m_FollowerInstance;

        protected PlayerController m_Target;

        protected float m_TimerSinceLostTarget;

        public EnemyController controller => m_Controller;

        public PlayerController target => m_Target;
        public TargetDistributor.TargetFollower followerData => m_FollowerInstance;

        public Vector3 originalPosition { get; protected set; }

        private void FixedUpdate()
        {
            m_Controller.animator.SetBool(hashGrounded, controller.grounded);

            var toBase = originalPosition - transform.position;
            toBase.y = 0;

            m_Controller.animator.SetBool(hashNearBase, toBase.sqrMagnitude < 0.1 * 0.1f);
        }

        protected void OnEnable()
        {
            m_Controller = GetComponentInChildren<EnemyController>();

            originalPosition = transform.position;

            meleeWeapon.SetOwner(gameObject);

            m_Controller.animator.Play(hashIdleState, 0, Random.value);

            SceneLinkedSMB<ChomperBehavior>.Initialise(m_Controller.animator, this);
        }

        protected void OnDisable()
        {
            if (m_FollowerInstance != null)
                m_FollowerInstance.distributor.UnregisterFollower(m_FollowerInstance);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            playerScanner.EditorGizmo(transform);
        }
#endif

        public void OnReceiveMessage(MessageType type, object sender, object msg)
        {
            switch (type)
            {
                case MessageType.DEAD:
                    Death((Damageable.DamageMessage)msg);
                    break;
                case MessageType.DAMAGED:
                    ApplyDamage((Damageable.DamageMessage)msg);
                    break;
            }
        }

        /// <summary>
        ///     Called by animation events.
        /// </summary>
        /// <param name="frontFoot">Has a value of 1 when it's a front foot stepping and 0 when it's a back foot.</param>
        private void PlayStep(int frontFoot)
        {
            if (frontStepAudio != null && frontFoot == 1)
                frontStepAudio.PlayRandomClip();
            else if (backStepAudio != null && frontFoot == 0)
                backStepAudio.PlayRandomClip();
        }

        /// <summary>
        ///     Called by animation events.
        /// </summary>
        public void Grunt()
        {
            if (gruntAudio != null)
                gruntAudio.PlayRandomClip();
        }

        public void Spotted()
        {
            if (spottedAudio != null)
                spottedAudio.PlayRandomClip();
        }

        public void FindTarget()
        {
            //we ignore height difference if the target was already seen
            var target = playerScanner.Detect(transform, m_Target == null);

            if (m_Target == null)
            {
                //we just saw the player for the first time, pick an empty spot to target around them
                if (target != null)
                {
                    m_Controller.animator.SetTrigger(hashSpotted);
                    m_Target = target;
                    var distributor = target.GetComponentInChildren<TargetDistributor>();
                    if (distributor != null)
                        m_FollowerInstance = distributor.RegisterNewFollower();
                }
            }
            else
            {
                //we lost the target. But chomper have a special behaviour : they only loose the player scent if they move past their detection range
                //and they didn't see the player for a given time. Not if they move out of their detectionAngle. So we check that this is the case before removing the target
                if (target == null)
                {
                    m_TimerSinceLostTarget += Time.deltaTime;

                    if (m_TimerSinceLostTarget >= timeToStopPursuit)
                    {
                        var toTarget = m_Target.transform.position - transform.position;

                        if (toTarget.sqrMagnitude > playerScanner.detectionRadius * playerScanner.detectionRadius)
                        {
                            if (m_FollowerInstance != null)
                                m_FollowerInstance.distributor.UnregisterFollower(m_FollowerInstance);

                            //the target move out of range, reset the target
                            m_Target = null;
                        }
                    }
                }
                else
                {
                    if (target != m_Target)
                    {
                        if (m_FollowerInstance != null)
                            m_FollowerInstance.distributor.UnregisterFollower(m_FollowerInstance);

                        m_Target = target;

                        var distributor = target.GetComponentInChildren<TargetDistributor>();
                        if (distributor != null)
                            m_FollowerInstance = distributor.RegisterNewFollower();
                    }

                    m_TimerSinceLostTarget = 0.0f;
                }
            }
        }

        public void StartPursuit()
        {
            if (m_FollowerInstance != null)
            {
                m_FollowerInstance.requireSlot = true;
                RequestTargetPosition();
            }

            m_Controller.animator.SetBool(hashInPursuit, true);
        }

        public void StopPursuit()
        {
            if (m_FollowerInstance != null) m_FollowerInstance.requireSlot = false;

            m_Controller.animator.SetBool(hashInPursuit, false);
        }

        public void RequestTargetPosition()
        {
            var fromTarget = transform.position - m_Target.transform.position;
            fromTarget.y = 0;

            m_FollowerInstance.requiredPoint =
                m_Target.transform.position + fromTarget.normalized * attackDistance * 0.9f;
        }

        public void WalkBackToBase()
        {
            if (m_FollowerInstance != null)
                m_FollowerInstance.distributor.UnregisterFollower(m_FollowerInstance);
            m_Target = null;
            StopPursuit();
            m_Controller.SetTarget(originalPosition);
            m_Controller.SetFollowNavmeshAgent(true);
        }

        public void TriggerAttack()
        {
            m_Controller.animator.SetTrigger(hashAttack);
        }

        public void AttackBegin()
        {
            meleeWeapon.BeginAttack(false);
        }

        public void AttackEnd()
        {
            meleeWeapon.EndAttack();
        }

        public void Death(Damageable.DamageMessage msg)
        {
            var pushForce = transform.position - msg.damageSource;

            pushForce.y = 0;

            transform.forward = -pushForce.normalized;
            controller.AddForce(pushForce.normalized * 7.0f - Physics.gravity * 0.6f);

            controller.animator.SetTrigger(hashHit);
            controller.animator.SetTrigger(hashThrown);

            //We unparent the hit source, as it would destroy it with the gameobject when it get replaced by the ragdol otherwise
            deathAudio.transform.SetParent(null, true);
            deathAudio.PlayRandomClip();
            Destroy(deathAudio, deathAudio.clip == null ? 0.0f : deathAudio.clip.length + 0.5f);
        }

        public void ApplyDamage(Damageable.DamageMessage msg)
        {
            //TODO : make that more generic, (e.g. move it to the MeleeWeapon code with a boolean to enable shaking of camera on hit?)
            if (msg.damager.name == "Staff")
                CameraShake.Shake(0.06f, 0.1f);

            var verticalDot = Vector3.Dot(Vector3.up, msg.direction);
            var horizontalDot = Vector3.Dot(transform.right, msg.direction);

            var pushForce = transform.position - msg.damageSource;

            pushForce.y = 0;

            transform.forward = -pushForce.normalized;
            controller.AddForce(pushForce.normalized * 5.5f, false);

            controller.animator.SetFloat(hashVerticalDot, verticalDot);
            controller.animator.SetFloat(hashHorizontalDot, horizontalDot);

            controller.animator.SetTrigger(hashHit);

            hitAudio.PlayRandomClip();
        }
    }
}