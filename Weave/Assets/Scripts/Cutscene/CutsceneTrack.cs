using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackColor(0.3f, 0.8f, 1f)]
[TrackClipType(typeof(CutsceneClip))]
public class CutsceneTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        // 不做混合，给个空 mixer
        return ScriptPlayable<TimelinePauseMixer>.Create(graph, inputCount);
    }
}

// 空的 mixer behaviour
public class TimelinePauseMixer : PlayableBehaviour { }