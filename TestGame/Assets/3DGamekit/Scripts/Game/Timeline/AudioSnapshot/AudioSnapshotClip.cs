using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class AudioSnapshotClip : PlayableAsset, ITimelineClipAsset
{
    public ExposedReference<AudioSource> audioSource;
    public AudioSnapshotBehaviour template = new AudioSnapshotBehaviour();

    public ClipCaps clipCaps => ClipCaps.All;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<AudioSnapshotBehaviour>.Create(graph, template);
        var clone = playable.GetBehaviour();
        clone.audioSource = audioSource.Resolve(graph.GetResolver());
        return playable;
    }
}