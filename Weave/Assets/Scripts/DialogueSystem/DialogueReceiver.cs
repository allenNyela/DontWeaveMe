using UnityEngine;

public class DialogueReceiver : MonoBehaviour
{
    public void SetValue(int value)
    {
        DialogueManager.Instance.StartDialogue(value);
    }
}
