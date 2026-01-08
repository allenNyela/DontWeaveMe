using UnityEngine;
using UnityEngine.Playables;

public class DialogueMixer : PlayableBehaviour
{
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        // If multiple clips overlap, Timeline blends them.
        // Here we just use the first active clip¡¯s value.

        var receiver = playerData as DialogueReceiver;
        if (receiver == null) return;

        int clipValue = 0;
        bool found = false;

        int count = playable.GetInputCount();
        for (int i = 0; i < count; i++)
        {
            float weight = playable.GetInputWeight(i);
            if (weight > 0f)
            {
                var inputPlayable = (ScriptPlayable<DialogueBehaviour>)playable.GetInput(i);
                var behaviour = inputPlayable.GetBehaviour();
                clipValue = behaviour.value;
                found = true;
                break;
            }
        }

        //if (found)
        //    receiver.SetValue(clipValue);
    }
}
