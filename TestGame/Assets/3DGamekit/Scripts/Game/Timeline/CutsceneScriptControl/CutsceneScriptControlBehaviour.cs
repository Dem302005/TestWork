using System;
using UnityEngine.Playables;

[Serializable]
public class CutsceneScriptControlBehaviour : PlayableBehaviour
{
    public bool playerInputEnabled;
    public bool useRootMotion;
    public PlayerInput playerInput;

    public override void OnGraphStart(Playable playable)
    {
    }
}