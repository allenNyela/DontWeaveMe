using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class DialogueData : ScriptableObject
{
    [SerializeField]
    public int CharacterId;

    [SerializeField]
    public List<DialogueLine> Dialogues;

    [SerializeField]
    public int Id;

    [Serializable]
    public class DialogueLine
    {
        public string line;
        public ConditionCheck conditionCheck;
    }
    
    public enum ConditionCheck
    {
        None = 0,
        WASD_Check = 1,
        ConnectNodeCheck = 2,
        FormWeb = 3,
        CancelNode = 4,
        RedoNode = 5,
    }
}
