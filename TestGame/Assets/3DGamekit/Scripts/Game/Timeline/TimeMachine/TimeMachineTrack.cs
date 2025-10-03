using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackColor(0.7366781f, 0.3261246f, 0.8529412f)]
[TrackClipType(typeof(TimeMachineClip))]
public class TimeMachineTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        var scriptPlayable = ScriptPlayable<TimeMachineMixerBehaviour>.Create(graph, inputCount);

        var b = scriptPlayable.GetBehaviour();
        b.markerClips = new Dictionary<string, double>();


        foreach (var c in GetClips())
        {
            var clip = (TimeMachineClip)c.asset;
            var clipName = c.displayName;

            switch (clip.action)
            {
                case TimeMachineBehaviour.TimeMachineAction.Pause:
                    clipName = "||";
                    break;

                case TimeMachineBehaviour.TimeMachineAction.Marker:
                    clipName = "● " + clip.markerLabel;

                    //Insert the marker clip into the Dictionary of markers
                    if (!b.markerClips.ContainsKey(clip
                            .markerLabel)) //happens when you duplicate a clip and it has the same markerLabel
                        b.markerClips.Add(clip.markerLabel, c.start);
                    break;

                case TimeMachineBehaviour.TimeMachineAction.JumpToMarker:
                    clipName = "↩︎  " + clip.markerToJumpTo;
                    break;

                case TimeMachineBehaviour.TimeMachineAction.JumpToTime:
                    clipName = "↩ " + clip.timeToJumpTo;
                    break;
            }

            c.displayName = clipName;


            if (clip.action == TimeMachineBehaviour.TimeMachineAction.Marker)
                if (!b.markerClips.ContainsKey(clip
                        .markerLabel)) //happens when you duplicate a clip and it has the same markerLabel
                    b.markerClips.Add(clip.markerLabel, c.start);
        }

        return scriptPlayable;
    }
}