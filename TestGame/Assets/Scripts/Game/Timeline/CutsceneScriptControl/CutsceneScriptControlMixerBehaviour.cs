using UnityEngine;
using UnityEngine.Playables;

public class CutsceneScriptControlMixerBehaviour : PlayableBehaviour
{
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        var inputCount = playable.GetInputCount();

        for (var i = 0; i < inputCount; i++)
        {
            var inputWeight = playable.GetInputWeight(i);

            if (!Mathf.Approximately(inputWeight, 1f))
                continue;

            var inputPlayable = (ScriptPlayable<CutsceneScriptControlBehaviour>)playable.GetInput(i);
            var input = inputPlayable.GetBehaviour();

            input.playerInput.enabled = input.playerInputEnabled;
        }
    }
}