using UnityEngine;
using UnityEngine.Playables;

public class SceneReloaderMixerBehaviour : PlayableBehaviour
{
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        var trackBinding = playerData as GameObject;

        if (trackBinding == null)
            return;

        var inputCount = playable.GetInputCount();

        for (var i = 0; i < inputCount; i++)
        {
            var inputWeight = playable.GetInputWeight(i);
            var inputPlayable = (ScriptPlayable<SceneReloaderBehaviour>)playable.GetInput(i);
            var input = inputPlayable.GetBehaviour();

            if (Mathf.Approximately(inputWeight, 1f) && Application.isPlaying) input.ReloadScene(trackBinding);
        }
    }
}