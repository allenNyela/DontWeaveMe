using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class GameLevelConfig : GameConfig
{
    [SerializeField]
    public DayNight Time; //different logic based on time

    [SerializeField]
    public string ScenePath; //loadscene

    [SerializeField]
    public bool CreateMother; //mother create in the scene

    [SerializeField]
    public int PlayerStamina;

    [SerializeField]
    public int MotherStamina;

    [SerializeField]
    public int PlayerAvailableNodes;

    [SerializeField]
    public Vector2 MotherPosition;

    [SerializeField]
    public Vector2 PlayerPosition;

    [SerializeField]
    public int FlyQuota;      // how many flies must be eaten to clear this level

    [SerializeField]
    public bool PlayTutorial;

    [SerializeField]
    public bool HideSleepButton = false;

    [SerializeField]
    public List<Vector2Int> lockNodes;
}
