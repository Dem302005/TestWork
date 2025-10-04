using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimeMachineMixerBehaviour : PlayableBehaviour
{
    public Dictionary<string, double> markerClips;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        //ScriptPlayable<TimeMachineBehaviour> inputPlayable = (ScriptPlayable<TimeMachineBehaviour>)playable.GetInput(i);
        //Debug.Log(PlayableExtensions.GetTime<ScriptPlayable<TimeMachineBehaviour>>(inputPlayable));

        if (!Application.isPlaying) return;

        var inputCount = playable.GetInputCount();

        for (var i = 0; i < inputCount; i++)
        {
            var inputWeight = playable.GetInputWeight(i);
            var inputPlayable = (ScriptPlayable<TimeMachineBehaviour>)playable.GetInput(i);
            var input = inputPlayable.GetBehaviour();

            if (inputWeight > 0f)
                switch (input.action)
                {
                    case TimeMachineBehaviour.TimeMachineAction.Pause:
                        Debug.Log("Pause");
                        (playable.GetGraph().GetResolver() as PlayableDirector).Pause();
                        break;

                    case TimeMachineBehaviour.TimeMachineAction.JumpToTime:
                    case TimeMachineBehaviour.TimeMachineAction.JumpToMarker:
                        if (input.ConditionMet())
                        {
                            //Rewind
                            if (input.action == TimeMachineBehaviour.TimeMachineAction.JumpToTime)
                            {
                                //Jump to time
                                (playable.GetGraph().GetResolver() as PlayableDirector).time = input.timeToJumpTo;
                            }
                            else
                            {
                                //Jump to marker
                                var t = markerClips[input.markerToJumpTo];
                                (playable.GetGraph().GetResolver() as PlayableDirector).time = t;
                            }
                        }

                        break;
                }
        }
    }
}