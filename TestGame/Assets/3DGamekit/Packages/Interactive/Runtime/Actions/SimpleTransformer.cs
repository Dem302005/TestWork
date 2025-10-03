using UnityEngine;

namespace Gamekit3D.GameCommands
{
    public abstract class SimpleTransformer : GameCommandHandler
    {
        public enum LoopType
        {
            Once,
            PingPong,
            Repeat
        }

        public LoopType loopType;

        public float duration = 1;
        public AnimationCurve accelCurve;

        public bool activate;
        public SendGameCommand OnStartCommand, OnStopCommand;

        public AudioSource onStartAudio, onEndAudio;

        [Range(0, 1)] public float previewPosition;

        private float direction = 1f;

        protected Platform m_Platform;
        private float position;
        private float time;

        protected override void Awake()
        {
            base.Awake();

            m_Platform = GetComponentInChildren<Platform>();
        }

        public void FixedUpdate()
        {
            if (activate)
            {
                time = time + direction * Time.deltaTime / duration;
                switch (loopType)
                {
                    case LoopType.Once:
                        LoopOnce();
                        break;
                    case LoopType.PingPong:
                        LoopPingPong();
                        break;
                    case LoopType.Repeat:
                        LoopRepeat();
                        break;
                }

                PerformTransform(position);
            }
        }

        [ContextMenu("Test Start Audio")]
        private void TestPlayAudio()
        {
            if (onStartAudio != null) onStartAudio.Play();
        }

        public override void PerformInteraction()
        {
            activate = true;
            if (OnStartCommand != null) OnStartCommand.Send();
            if (onStartAudio != null) onStartAudio.Play();
        }

        public virtual void PerformTransform(float position)
        {
        }

        private void LoopPingPong()
        {
            position = Mathf.PingPong(time, 1f);
        }

        private void LoopRepeat()
        {
            position = Mathf.Repeat(time, 1f);
        }

        private void LoopOnce()
        {
            position = Mathf.Clamp01(time);
            if (position >= 1)
            {
                enabled = false;
                if (OnStopCommand != null) OnStopCommand.Send();
                direction *= -1;
            }
        }
    }
}