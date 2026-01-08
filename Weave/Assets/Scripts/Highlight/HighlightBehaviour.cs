using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public class HighlightBehaviour : PlayableBehaviour
{
    public List<HighlightEnum> targets;
    private bool hasHighlighted;
    private bool hasUnhighlighted;
    public double clipDuration;
    public override void PrepareFrame(Playable playable, FrameData info)
    {
        double t = playable.GetTime();
        double duration = playable.GetDuration();


        // 1. Clip 开始播放（只触发一次）
        if (!hasHighlighted && t > 0)
        {
            hasHighlighted = true;
            Highlight.Instance.HighlightByType(targets);
            Debug.Log($"[Highlight] ENTER, mode={PrintEnums(targets)}, t={t:F3}");
        }

        //// 2. Clip 到达结尾（只触发一次）
        //if (!hasUnhighlighted && duration > 0.0 && t >= duration - 0.0001)
        //{
        //    Debug.Log($"[Highlight] EXIT, mode={targets}, t={t:F3}, duration={duration:F3}");
        //    hasUnhighlighted = true;
        //    Highlight.Instance.UnhighlightByType(targets);
        //}
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        double t = playable.GetTime();
        var duration = playable.GetDuration();
        var time = playable.GetTime();
        var count = time + info.deltaTime;

        if ((info.effectivePlayState == PlayState.Paused && count > duration) || Mathf.Approximately((float)time, (float)duration))
        {
            // Execute your finishing logic here:
            Highlight.Instance.UnhighlightByType(targets);
            hasUnhighlighted = true;
            Debug.Log($"[Highlight] Exit, mode={PrintEnums(targets)}, t={t:F3}");
        }
    }

    private string PrintEnums(List<HighlightEnum> lst)
    {
        return "[" + string.Join(", ", lst) + "]";
    }
}
