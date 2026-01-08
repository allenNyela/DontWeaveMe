using UnityEngine;
using UnityEngine.Playables;

public class DialogueBehaviour : PlayableBehaviour
{
    public int value;
    private bool hasFired;

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        //hasFired = false;
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        var receiver = playerData as DialogueReceiver;
        if (receiver == null) return;

        if (!hasFired)
        {
            receiver.SetValue(value);
            hasFired = true;
        }
    }


    public override void OnBehaviourPause(Playable playable, FrameData info)
    {

    }

}
