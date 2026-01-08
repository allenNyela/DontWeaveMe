using System.Collections;
using UnityEngine;

public class CutsceneState : GameState
{

    public CutsceneState(string scenePath) : base(scenePath)
    {

    }

    public override void OnEnter()
    {
        base.OnEnter();
        GameManager.Instance.FadeOut();
        GameManager.Instance.TurnOffGameUI();
    }
}
