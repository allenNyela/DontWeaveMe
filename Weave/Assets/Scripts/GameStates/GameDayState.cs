using System.Collections;
using UnityEngine;

public class GameDayState : GameState
{
    public GameDayState(string scenePath) : base(scenePath)
    {

    }

    public override void OnEnter()
    {
        base.OnEnter();
        GameManager.Instance.currentTime = DayNight.Day;
        GameManager.Instance.TurnOnGameUI();
        GameManager.Instance.RecoverBoardData();
        GameManager.Instance.CreateGame(gameConfig as GameLevelConfig);
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    public override void OnSceneLoaded(AsyncOperation ao)
    {
        base.OnSceneLoaded(ao);
        //GameManager.Instance.CreateGame(gameConfig as GameLevelConfig);
    }

}
