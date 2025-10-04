using System;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public class TimeMachineBehaviour : PlayableBehaviour
{
    public enum Condition
    {
        Always,
        Never
    }

    public enum TimeMachineAction
    {
        Marker,
        JumpToTime,
        JumpToMarker,
        Pause
    }

    public TimeMachineAction action;
    public Condition condition;
    public string markerToJumpTo, markerLabel;
    public float timeToJumpTo;

    [HideInInspector] public bool clipExecuted; //the user shouldn't author this, the Mixer does

    public bool ConditionMet()
    {
        switch (condition)
        {
            case Condition.Always:
                return true;

            case Condition.Never:
            default:
                return false;
        }
    }
}