using UnityEngine;
using UnityEngine.Playables;

public class SetLocationMixerBehaviour : PlayableBehaviour
{
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        var trackBinding = playerData as Transform;

        if (trackBinding == null)
            return;

        var inputCount = playable.GetInputCount();

        for (var i = 0; i < inputCount; i++)
        {
            var inputWeight = playable.GetInputWeight(i);
            var inputPlayable = (ScriptPlayable<SetLocationBehaviour>)playable.GetInput(i);
            var input = inputPlayable.GetBehaviour();

            if (Mathf.Approximately(inputWeight, 1f))
            {
                trackBinding.position = input.position;
                trackBinding.eulerAngles = input.eulerAngles;
            }
        }
    }
}