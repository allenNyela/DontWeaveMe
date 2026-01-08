using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class CutsceneClip : PlayableAsset, ITimelineClipAsset
{
    public ClipCaps clipCaps => ClipCaps.None;

    public TimelineWaitType WaitType;


    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<CutsceneBehaviour>.Create(graph);
        var b = playable.GetBehaviour();

        b.WaitType = WaitType;

        return playable;
    }
}
