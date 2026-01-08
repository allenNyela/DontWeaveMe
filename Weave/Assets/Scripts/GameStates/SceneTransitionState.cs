using System.Collections;
using UnityEngine;
using fsm;
using UnityEngine.SceneManagement;

public class SceneTransitionState : IState
{
    public string prevScenePath;
    public string nextScenePath;
    
    public SceneTransitionState(GameConfig prevConfig, GameConfig nextConfig)
    {
        prevScenePath = GetScenePath(prevConfig);
        nextScenePath = GetScenePath(nextConfig);
    }

    private string GetScenePath(GameConfig config)
    {
        if (config is GameLevelConfig gameConfig)
        {
            return gameConfig.ScenePath;
        }
        else if (config is CutsceneConfig cutConifg)
        {
            return cutConifg.ScenePath;
        }
        return string.Empty;
    }

    public void LateUpdate()
    {

    }

    public void OnEnter()
    {
        // => 
        GameManager.Instance.FadeIn(() =>
        {
            SceneManager.UnloadSceneAsync(prevScenePath).completed += OnSceneUnloaded;
        });
    }

    private void OnSceneUnloaded(AsyncOperation ao)
    {
        SceneManager.LoadSceneAsync(nextScenePath, LoadSceneMode.Additive).completed += OnSceneLoaded;
    }

    private void OnSceneLoaded(AsyncOperation ao)
    {
        GameManager.Instance.stateMachine.Trigger<StateChangeTrigger>(); //get the hell out of here
    }


    public void OnExit()
    {

    }

    public void Update()
    {

    }

    public void UpdateState(float deltaTime)
    {

    }
}
