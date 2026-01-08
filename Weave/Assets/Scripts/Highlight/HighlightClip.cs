using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Linq;


[Serializable]
public class HighlightClip : PlayableAsset, ITimelineClipAsset {

    public HighlightBehaviour template = new HighlightBehaviour();
    public List<HighlightEnum> targets;
    // Clip 的能力，目前不需要混合，所以 None 就行
    public ClipCaps clipCaps => ClipCaps.None;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<HighlightBehaviour>.Create(graph, template);

        HighlightBehaviour behaviour = playable.GetBehaviour();
        behaviour.clipDuration = this.duration;
        behaviour.targets = targets.ToList();

        return playable;
    }
}
