using System.Collections;
using UnityEngine;

public class GameNightState : GameState
{
    public GameNightState(string scenePath) : base(scenePath)
    {

    }

    public override void OnEnter()
    {
        base.OnEnter();
        GameManager.Instance.currentTime = DayNight.Night;
        GameManager.Instance.TurnOnGameUI();

        GameManager.Instance.CreateGame(gameConfig as GameLevelConfig);
    }

    public override void OnExit()
    {
        base.OnExit();
        GameManager.Instance.SaveBoardData();
    }

    public override void OnSceneLoaded(AsyncOperation ao)
    {
        base.OnSceneLoaded(ao);
        //GameManager.Instance.CreateGame(gameConfig as GameLevelConfig);
    }

}