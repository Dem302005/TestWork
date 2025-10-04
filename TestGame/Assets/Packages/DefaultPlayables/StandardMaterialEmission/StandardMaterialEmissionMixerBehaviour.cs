using UnityEngine;
using UnityEngine.Playables;

public class StandardMaterialEmissionMixerBehaviour : PlayableBehaviour
{
    private const string k_EmissionColorName = "_EmissionColor";
    private Color m_DefaultColor;
    private int m_EmissionColorId;

    private bool m_FirstFrameHappened;
    private bool m_IndicesMatch = true;
    private int m_MaterialIndex = -1;
    private Renderer m_TrackBinding;
    private Material[] m_TrackBindingMaterials;

    public override void OnGraphStart(Playable playable)
    {
        m_EmissionColorId = Shader.PropertyToID(k_EmissionColorName);
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        m_TrackBinding = playerData as Renderer;

        if (m_TrackBinding == null)
            return;

        var inputCount = playable.GetInputCount();

        if (!m_FirstFrameHappened)
            for (var i = 0; i < inputCount; i++)
            {
                var inputPlayable = (ScriptPlayable<StandardMaterialEmissionBehaviour>)playable.GetInput(i);
                var input = inputPlayable.GetBehaviour();
                if (i == 0)
                {
                    m_MaterialIndex = input.materialIndex;
                }
                else if (m_MaterialIndex < 0 || m_MaterialIndex != input.materialIndex)
                {
                    m_IndicesMatch = false;
                    for (var j = 0; j < inputCount; j++)
                    {
                        var checkedInputPlayable =
                            (ScriptPlayable<StandardMaterialEmissionBehaviour>)playable.GetInput(j);
                        var checkedInput = checkedInputPlayable.GetBehaviour();
                        checkedInput.materialIndicesMatch = false;
                    }

                    break;
                }
            }

        if (!m_IndicesMatch)
            return;

        if (!m_FirstFrameHappened)
        {
            m_TrackBindingMaterials = new Material[m_TrackBinding.sharedMaterials.Length];
            m_TrackBindingMaterials[m_MaterialIndex] = new Material(m_TrackBinding.sharedMaterials[m_MaterialIndex]);
            m_TrackBinding.materials = m_TrackBindingMaterials;
            m_DefaultColor = m_TrackBindingMaterials[m_MaterialIndex].GetColor(m_EmissionColorId);
            m_FirstFrameHappened = true;
        }

        var blendedColor = Color.clear;

        for (var i = 0; i < inputCount; i++)
        {
            var inputWeight = playable.GetInputWeight(i);
            var inputPlayable = (ScriptPlayable<StandardMaterialEmissionBehaviour>)playable.GetInput(i);
            var input = inputPlayable.GetBehaviour();

            blendedColor += input.color * inputWeight;
        }

        m_TrackBindingMaterials[m_MaterialIndex].SetColor(m_EmissionColorId, blendedColor);
    }

    public override void OnGraphStop(Playable playable)
    {
        m_TrackBindingMaterials[m_MaterialIndex].SetColor(m_EmissionColorId, m_DefaultColor);
        Object.Destroy(m_TrackBindingMaterials[m_MaterialIndex]);
        m_FirstFrameHappened = false;
    }
}