using UnityEngine;
using UnityEngine.Playables;

public class MaterialSwitcherMixerBehaviour : PlayableBehaviour
{
    private Material[] m_DefaultMaterials;
    private bool m_FirstFrameHappened;
    private int m_InputCount = -1;
    private Material[] m_OriginalSharedMaterials;
    private Renderer m_TrackBinding;

    public override void OnPlayableCreate(Playable playable)
    {
        m_InputCount = playable.GetInputCount();
    }

    private bool Setup(Playable playable)
    {
        m_OriginalSharedMaterials = m_TrackBinding.sharedMaterials;
        m_DefaultMaterials = new Material[m_OriginalSharedMaterials.Length];
        for (var i = 0; i < m_OriginalSharedMaterials.Length; i++)
            m_DefaultMaterials[i] = new Material(m_OriginalSharedMaterials[i]);

        if (m_InputCount > 0)
        {
            for (var i = 0; i < m_InputCount; i++)
            {
                var inputPlayable = (ScriptPlayable<MaterialSwitcherBehaviour>)playable.GetInput(i);
                var input = inputPlayable.GetBehaviour();
                if (!input.SetMaterials(m_DefaultMaterials)) return false;
            }

            return true;
        }

        return false;
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        m_TrackBinding = playerData as Renderer;

        if (m_TrackBinding == null)
            return;

        if (!m_FirstFrameHappened)
        {
            m_FirstFrameHappened = m_DefaultMaterials != null;
            m_FirstFrameHappened &= Setup(playable);
        }

        if (!m_FirstFrameHappened)
            return;

        m_TrackBinding.materials = m_DefaultMaterials;

        for (var i = 0; i < m_InputCount; i++)
        {
            var inputWeight = playable.GetInputWeight(i);
            var inputPlayable = (ScriptPlayable<MaterialSwitcherBehaviour>)playable.GetInput(i);
            var input = inputPlayable.GetBehaviour();

            if (inputWeight > 0 && input.setupCorrectly) m_TrackBinding.materials = input.materials;
        }
    }

    public override void OnPlayableDestroy(Playable playable)
    {
        if (m_FirstFrameHappened)
        {
            m_FirstFrameHappened = false;

            if (m_DefaultMaterials != null)
            {
                if (m_TrackBinding != null)
                    m_TrackBinding.sharedMaterials = m_OriginalSharedMaterials;

                m_DefaultMaterials = null;
            }
        }
    }
}