using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SequenceDatabase : ScriptableObject
{
    [Serializable]
    public class SequenceSlot
    {
        public GameConfig CurrentState;
        public GameConfig FailState;
    }

    [SerializeField]
    public List<SequenceSlot> GameFlowSequences; //GameFlow

}
