using fsm;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameState : IState
{
    protected string scenePath;
    protected GameConfig gameConfig;

    public GameState(string inScenePath)
    {
        scenePath = inScenePath;
    }

    public virtual void LateUpdate()
    {

    }

    public virtual void OnEnter()
    {
        GameManager.Instance.SetConfig(gameConfig);
        //SceneManager.LoadSceneAsync(scenePath, LoadSceneMode.Additive).completed += OnSceneLoaded;
    }

    public virtual void OnExit()
    {
        //GameManager.Instance.FadeIn(() =>
        //{
        //    SceneManager.UnloadSceneAsync(scenePath);
        //});

    }

    public virtual void Update()
    {

    }

    public virtual void UpdateState(float deltaTime)
    {

    }

    public virtual void OnSceneLoaded(AsyncOperation ao)
    {

    }

    public void SetConfig(GameConfig inConfig)
    {
        gameConfig = inConfig;
    }

    public GameConfig GetConfig()
    {
        return gameConfig;
    }
}
