using UnityEngine;
using UnityEngine.Playables;

public class CutsceneBehaviour : PlayableBehaviour
{
    public TimelineWaitType WaitType;

    private bool hasTrigger = false;
    public override void PrepareFrame(Playable playable, FrameData info)
    {
        double t = playable.GetTime();
        double duration = playable.GetDuration();


        // 1. Clip 开始播放（只触发一次）
        if (!hasTrigger && t > 0)
        {
            hasTrigger = true;
            CutsceneManager.Instance.PauseTimelineAndCheck(WaitType);
        }

    }
}
