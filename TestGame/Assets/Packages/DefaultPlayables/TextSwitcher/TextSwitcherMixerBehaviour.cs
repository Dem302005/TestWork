using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class TextSwitcherMixerBehaviour : PlayableBehaviour
{
    private Color m_DefaultColor;
    private int m_DefaultFontSize;
    private string m_DefaultText;
    private bool m_FirstFrameHappened;

    private Text m_TrackBinding;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        m_TrackBinding = playerData as Text;

        if (m_TrackBinding == null)
            return;

        if (!m_FirstFrameHappened)
        {
            m_DefaultColor = m_TrackBinding.color;
            m_DefaultFontSize = m_TrackBinding.fontSize;
            m_DefaultText = m_TrackBinding.text;
            m_FirstFrameHappened = true;
        }

        var inputCount = playable.GetInputCount();

        var blendedColor = Color.clear;
        var blendedFontSize = 0f;
        var totalWeight = 0f;
        var greatestWeight = 0f;
        var currentInputs = 0;

        for (var i = 0; i < inputCount; i++)
        {
            var inputWeight = playable.GetInputWeight(i);
            var inputPlayable = (ScriptPlayable<TextSwitcherBehaviour>)playable.GetInput(i);
            var input = inputPlayable.GetBehaviour();

            blendedColor += input.color * inputWeight;
            blendedFontSize += input.fontSize * inputWeight;
            totalWeight += inputWeight;

            if (inputWeight > greatestWeight)
            {
                m_TrackBinding.text = input.text;
                greatestWeight = inputWeight;
            }

            if (!Mathf.Approximately(inputWeight, 0f))
                currentInputs++;
        }

        m_TrackBinding.color = blendedColor + m_DefaultColor * (1f - totalWeight);
        m_TrackBinding.fontSize = Mathf.RoundToInt(blendedFontSize + m_DefaultFontSize * (1f - totalWeight));
        if (currentInputs != 1 && 1f - totalWeight > greatestWeight) m_TrackBinding.text = m_DefaultText;
    }

    public override void OnGraphStop(Playable playable)
    {
        m_TrackBinding.color = m_DefaultColor;
        m_TrackBinding.fontSize = m_DefaultFontSize;
        m_TrackBinding.text = m_DefaultText;
        m_FirstFrameHappened = false;
    }
}