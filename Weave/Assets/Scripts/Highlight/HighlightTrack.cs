using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;



[TrackColor(0.9f, 0.8f, 0.2f)]              // 轨道在 Timeline 里的颜色
[TrackClipType(typeof(HighlightClip))]       // 这条轨只允许放 HighlightClip
public class HighlightTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        // 不需要真正混合，给一个空 mixer 就行
        return ScriptPlayable<HighlightMixerBehaviour>.Create(graph, inputCount);
    }
}
