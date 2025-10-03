using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackColor(0.1088343f, 0.4485294f, 0.2939095f)]
[TrackClipType(typeof(MaterialSwitcherClip))]
[TrackBindingType(typeof(Renderer))]
public class MaterialSwitcherTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<MaterialSwitcherMixerBehaviour>.Create(graph, inputCount);
    }

    public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
    {
#if UNITY_EDITOR
        var trackBinding = director.GetGenericBinding(this) as Renderer;
        if (trackBinding == null)
            return;

        var serializedObject = new SerializedObject(trackBinding);
        var iterator = serializedObject.GetIterator();
        while (iterator.NextVisible(true))
        {
            if (iterator.hasVisibleChildren)
                continue;

            driver.AddFromName<Light>(trackBinding.gameObject, iterator.propertyPath);
        }
#endif
        base.GatherProperties(director, driver);
    }
}