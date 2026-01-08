using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu]
public class DialogueDatabase : ScriptableObject
{
    [SerializeField]
    public List<DialogueData> Dialogs;

    [SerializeField]
    public List<DialogueCharacter> Characters;

    [HideInInspector]
    public Dictionary<int, DialogueData> DataBaseForDialogue;

    [HideInInspector]
    public Dictionary<int, DialogueCharacter> DataBaseForCharacters;


    public void Init()
    {
        DataBaseForDialogue = new Dictionary<int, DialogueData>();
        DataBaseForCharacters = new Dictionary<int, DialogueCharacter>();

        foreach(var d in Dialogs)
        {
            DataBaseForDialogue.Add(d.Id, d);
        }

        foreach(var c in Characters)
        {
            DataBaseForCharacters.Add(c.id, c);
        }
    }
}
