using UnityEngine;

namespace Gamekit3D
{
    public class EllenSpawn : MonoBehaviour
    {
        private const string k_BoundsName = "_bounds";
        private const string k_CutoffName = "_Cutoff";

        [HideInInspector] public float effectTime;

        public Material[] EllenRespawnMaterials;
        public GameObject respawnParticles;
        private Material[] EllenMaterials;
        private float m_EndTime;

        private MaterialPropertyBlock m_PropertyBlock;
        private Renderer m_Renderer;

        private bool m_Started;
        private float m_Timer;
        private Vector4 pos;
        private Vector3 renderBounds;

        private void Awake()
        {
            respawnParticles.SetActive(false);
            m_PropertyBlock = new MaterialPropertyBlock();
            m_Renderer = GetComponentInChildren<Renderer>();
            EllenMaterials = m_Renderer.materials;

            renderBounds = m_Renderer.bounds.size;
            pos.y = renderBounds.y;

            m_Renderer.GetPropertyBlock(m_PropertyBlock);
            m_PropertyBlock.SetVector(k_BoundsName, pos);
            m_PropertyBlock.SetFloat(k_CutoffName, 0.0001f);
            m_Renderer.SetPropertyBlock(m_PropertyBlock);

            pos = new Vector4(0, 0, 0, 0);

            m_Started = false;

            enabled = false;
        }

        private void Update()
        {
            if (!m_Started)
                return;

            var cutoff = Mathf.Clamp(m_Timer / effectTime, 0.01f, 1.0f);
            Set(cutoff);

            m_Timer += Time.deltaTime;

            if (cutoff >= 1.0f)
            {
                m_Renderer.materials = EllenMaterials;
                enabled = false;
            }
        }

        private void OnEnable()
        {
            m_Started = false;
            m_Renderer.materials = EllenRespawnMaterials;
            Set(0.001f);
            m_Renderer.enabled = false;
        }

        public void StartEffect()
        {
            m_Renderer.enabled = true;

            respawnParticles.SetActive(true);
            m_Started = true;
            m_Timer = 0.0f;
        }

        private void Set(float cutoff)
        {
            renderBounds = m_Renderer.bounds.size;
            pos.y = renderBounds.y;
            m_Renderer.GetPropertyBlock(m_PropertyBlock);
            m_PropertyBlock.SetVector(k_BoundsName, pos);

            m_PropertyBlock.SetFloat(k_CutoffName, cutoff);
            m_Renderer.SetPropertyBlock(m_PropertyBlock);
        }
    }
}