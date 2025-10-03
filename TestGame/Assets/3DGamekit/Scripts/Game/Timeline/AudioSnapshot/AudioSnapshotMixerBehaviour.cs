using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Playables;

public class AudioSnapshotMixerBehaviour : PlayableBehaviour
{
    private float[] m_CurrentWeights;
    private AudioMixer m_Mixer;
    private AudioMixerSnapshot[] m_Snapshots;

    public override void OnGraphStart(Playable playable)
    {
        var inputCount = playable.GetInputCount();

        m_Snapshots = new AudioMixerSnapshot[inputCount];
        m_CurrentWeights = new float[inputCount];

        for (var i = 0; i < inputCount; i++)
        {
            var inputPlayable = (ScriptPlayable<AudioSnapshotBehaviour>)playable.GetInput(i);
            var input = inputPlayable.GetBehaviour();

            m_Snapshots[i] = input.snapshot;
        }

        if (m_Snapshots.Length > 0)
            m_Mixer = m_Snapshots[0].audioMixer;
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        // First frame of each behaviour: play audio clip if given, play audio source if given
        var inputCount = playable.GetInputCount();

        for (var i = 0; i < inputCount; i++)
        {
            var inputWeight = playable.GetInputWeight(i);

            m_CurrentWeights[i] = inputWeight;

            var inputPlayable = (ScriptPlayable<AudioSnapshotBehaviour>)playable.GetInput(i);
            var input = inputPlayable.GetBehaviour();

            if (Application.isPlaying)
                input.PlayAudio(inputWeight);

            input.audioSource.volume = input.weightedVolume ? input.volume * playable.GetInputWeight(i) : input.volume;
        }

        if (m_Mixer != null)
            m_Mixer.TransitionToSnapshots(m_Snapshots, m_CurrentWeights, 0f);
    }
}