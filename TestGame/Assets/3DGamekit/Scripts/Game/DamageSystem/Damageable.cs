using System;
using System.Collections.Generic;
using Gamekit3D.Message;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using MessageType = Gamekit3D.Message.MessageType;

namespace Gamekit3D
{
    public partial class Damageable : MonoBehaviour
    {
        public int maxHitPoints;

        [Tooltip("Time that this gameObject is invulnerable for, after receiving damage.")]
        public float invulnerabiltyTime;


        [Tooltip(
            "The angle from the which that damageable is hitable. Always in the world XZ plane, with the forward being rotate by hitForwardRoation")]
        [Range(0.0f, 360.0f)]
        public float hitAngle = 360.0f;

        [Tooltip("Allow to rotate the world forward vector of the damageable used to define the hitAngle zone")]
        [Range(0.0f, 360.0f)]
        [FormerlySerializedAs("hitForwardRoation")]
        //SHAME!
        public float hitForwardRotation = 360.0f;

        public UnityEvent OnDeath, OnReceiveDamage, OnHitWhileInvulnerable, OnBecomeVulnerable, OnResetDamage;

        [Tooltip("When this gameObject is damaged, these other gameObjects are notified.")]
        [EnforceType(typeof(IMessageReceiver))]
        public List<MonoBehaviour> onDamageMessageReceivers;

        protected Collider m_Collider;

        protected float m_timeSinceLastHit;

        private Action schedule;

        public bool isInvulnerable { get; set; }
        public int currentHitPoints { get; private set; }

        private void Start()
        {
            ResetDamage();
            m_Collider = GetComponent<Collider>();
        }

        private void Update()
        {
            if (isInvulnerable)
            {
                m_timeSinceLastHit += Time.deltaTime;
                if (m_timeSinceLastHit > invulnerabiltyTime)
                {
                    m_timeSinceLastHit = 0.0f;
                    isInvulnerable = false;
                    OnBecomeVulnerable.Invoke();
                }
            }
        }

        private void LateUpdate()
        {
            if (schedule != null)
            {
                schedule();
                schedule = null;
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            var forward = transform.forward;
            forward = Quaternion.AngleAxis(hitForwardRotation, transform.up) * forward;

            if (Event.current.type == EventType.Repaint)
            {
                Handles.color = Color.blue;
                Handles.ArrowHandleCap(0, transform.position, Quaternion.LookRotation(forward), 1.0f,
                    EventType.Repaint);
            }


            Handles.color = new Color(1.0f, 0.0f, 0.0f, 0.5f);
            forward = Quaternion.AngleAxis(-hitAngle * 0.5f, transform.up) * forward;
            Handles.DrawSolidArc(transform.position, transform.up, forward, hitAngle, 1.0f);
        }
#endif

        public void ResetDamage()
        {
            currentHitPoints = maxHitPoints;
            isInvulnerable = false;
            m_timeSinceLastHit = 0.0f;
            OnResetDamage.Invoke();
        }

        public void SetColliderState(bool enabled)
        {
            m_Collider.enabled = enabled;
        }

        public void ApplyDamage(DamageMessage data)
        {
            if (currentHitPoints <= 0) return;

            if (isInvulnerable)
            {
                OnHitWhileInvulnerable.Invoke();
                return;
            }

            var forward = transform.forward;
            forward = Quaternion.AngleAxis(hitForwardRotation, transform.up) * forward;

            var positionToDamager = data.damageSource - transform.position;
            positionToDamager -= transform.up * Vector3.Dot(transform.up, positionToDamager);

            if (Vector3.Angle(forward, positionToDamager) > hitAngle * 0.5f)
                return;

            isInvulnerable = true;
            currentHitPoints -= data.amount;

            if (currentHitPoints <= 0)
                schedule += () =>
                {
                    OnDeath.Invoke();
                    var identifier = GetComponentInParent<EnemyIdentifier>();
                    if (identifier != null) EnemyCounter.IncrementKillCount(identifier.type);
                };
            else
                OnReceiveDamage.Invoke();

            var messageType = currentHitPoints <= 0 ? MessageType.DEAD : MessageType.DAMAGED;

            for (var i = 0; i < onDamageMessageReceivers.Count; ++i)
            {
                var receiver = onDamageMessageReceivers[i] as IMessageReceiver;
                receiver.OnReceiveMessage(messageType, this, data);
            }
        }
    }
}