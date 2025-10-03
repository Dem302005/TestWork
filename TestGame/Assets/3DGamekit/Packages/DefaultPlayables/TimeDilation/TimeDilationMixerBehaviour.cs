using UnityEngine;
using UnityEngine.Playables;

public class TimeDilationMixerBehaviour : PlayableBehaviour
{
    private float m_OldTimeScale = 1f;

    public override void OnPlayableCreate(Playable playable)
    {
        m_OldTimeScale = Time.timeScale;
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        var inputCount = playable.GetInputCount();

        var mixedTimeScale = 0f;
        var totalWeight = 0f;

        for (var i = 0; i < inputCount; i++)
        {
            var inputWeight = playable.GetInputWeight(i);

            totalWeight += inputWeight;

            var playableInput = (ScriptPlayable<TimeDilationBehaviour>)playable.GetInput(i);
            var input = playableInput.GetBehaviour();

            mixedTimeScale += inputWeight * input.timeScale;
        }

        Time.timeScale = mixedTimeScale + m_OldTimeScale * (1f - totalWeight);
    }

    public override void OnGraphStop(Playable playable)
    {
        Time.timeScale = m_OldTimeScale;
    }

    public override void OnPlayableDestroy(Playable playable)
    {
        Time.timeScale = m_OldTimeScale;
    }
}