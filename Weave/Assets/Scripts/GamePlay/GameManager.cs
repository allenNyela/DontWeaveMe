using fsm;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static WeaveBoardManager;
using DG.Tweening;
using System;
using static SequenceDatabase;
using UnityEngine.SceneManagement;
using Scene = UnityEngine.SceneManagement.Scene;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public SpiderController player;
    public SpiderController mother;

    public StateMachine stateMachine; //game statemachine

    public TMP_Text debugStateText;

    public bool isStartScene = false;

    public List<WebStruct> webData = new List<WebStruct>();

    private List<string> hasPlayedTutorials = new List<string>();

    public bool _recover = false;
    public List<WebStruct> _perserveData = new List<WebStruct>();

    public bool firstMeal = true;
    public bool firstWeave = true;

    public int motherMaxStamina = 50;
    public int playerMaxStamina = 10;
    public int numberOfNodesAvailable = 3;

    public GameObject playerPrefab;
    public GameObject motherPrefab;
    public RectTransform Canvas;
    private RectTransform GameUI;
    private RectTransform GameStatsUI;
    private RectTransform TransitionUI;
    public GameObject sleepButton;
    //public SilkBar playerEnergy;
    //public SilkBar motherEnergy;

    public DayNight currentTime;

    //game flow control
    public SequenceDatabase GameFlow;

    public RectTransform dialogue;

    public RectTransform pointer;

    public TMP_Text dialogueText;

    public Button endTurn;

    public GameObject DayStatus;
    public GameObject NightStatus;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!isStartScene)
            return;

        BindUI();
        //CreateStateMachine();
        CreateGameFlow();
    }

    private bool ThisTurnHasEnd = false;
    private void BindUI()
    {
        GameUI = Canvas.Find("GameUI").GetComponent<RectTransform>();
        GameStatsUI = GameUI.Find("GameStatsUI").GetComponent<RectTransform>();
        TransitionUI = Canvas.Find("TransitionUI").GetComponent<RectTransform>();

        //playerEnergy = GameStatsUI.Find("SpiderEnergyBar").GetComponent<EnergyBar>();
        //motherEnergy = GameStatsUI.Find("MotherSpiderEnergyBar").GetComponent<EnergyBar>();

        endTurn.onClick.RemoveAllListeners();
        endTurn.onClick.AddListener(() =>
        {
            if (stateMachine.CurrentState is GameNightState || stateMachine.CurrentState is GameDayState)
            {
                if (!ThisTurnHasEnd)
                {
                    bool trueTurn = EndTurn();
                    if (trueTurn)
                    {
                        ThisTurnHasEnd = true;
                        Debug.Log($"==>set turn true");
                    }
                }
            }

        });
    }

    public void ResetTurn()
    {
        ThisTurnHasEnd = false;
        Canvas.Find("GameUI/FailPanel").GetComponent<FailPanel>().ResetTrigger();
        Debug.Log($"==>reset turn");

    }

    private GameConfig gameConfig;
    public void SetConfig(GameConfig config)
    {
        gameConfig = config;
    }

    public void FadeIn(Action callback = null)
    {
        TransitionUI.Find("Fade").GetComponent<CanvasGroup>().DOFade(1f, 2f).onComplete += () =>
        {
            callback?.Invoke();
        };
    }

    public void FadeOut(Action callback = null)
    {
        TransitionUI.Find("Fade").GetComponent<CanvasGroup>().DOFade(0f, 2f).onComplete += () =>
        {
            callback?.Invoke();
        };
    }

    public void TurnOnGameUI()
    {
        GameUI.gameObject.SetActive(true);
        Debug.Log($"========> Turn On Game UI");
        TurnOnGameStatsUI();
    }

    public void TurnOffGameUI()
    {
        GameUI.gameObject.SetActive(false);
        Debug.Log($"========> Turn Off Game UI");
    }

    public void TurnOnGameStatsUI()
    {
        GameStatsUI.gameObject.SetActive(true);
        Debug.Log($"========> Turn On Game Stat");
    }

    public void TurnOffGameStatsUI()
    {
        GameStatsUI.gameObject.SetActive(false);
        Debug.Log($"========> Turn Off Game Stat");
    }


    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (stateMachine != null)
        {
            debugStateText.text = stateMachine.CurrentState.ToString();
        }
        //Debug.Log($"GameManager's current config: {CurrentConfig} + fly quota: {CurrentConfig?.FlyQuota}");

    }

    public void SaveBoardData()
    {
        webData = WeaveBoardManager.instance.GenerateWebData();
    }

    public void RecoverBoardData()
    {
        WeaveBoardManager.instance.RecoverBoard(webData);
    }

    /// <summary>
    /// Debug Use
    /// </summary>
    public void JumpSequence(int index)
    {
        if (stateMachine.CurrentState is SceneTransitionState)
            return;

        //FORCE LOAD
        var transitionState = new SceneTransitionState(gameConfig, GameFlow.GameFlowSequences[index].CurrentState);
        stateMachine.RegisterStates(transitionState);
        stateMachine.Configure(transitionState).AddTransition<StateChangeTrigger>(gameState[index]);

        stateMachine.ChangeState(transitionState);
    }

    private List<GameState> gameState = new List<GameState>();
    
    //complete game flow statemachine
    private void CreateGameFlow()
    {
        stateMachine = new StateMachine();
        gameState.Clear();
        Dictionary<GameConfig, GameState> tempStateDict = new Dictionary<GameConfig, GameState>();
        List<GameState> gameStates = new List<GameState>();

        foreach (var gameconfig in GameFlow.GameFlowSequences)
        {
            GameState currentGameState = null;
            if (gameconfig.CurrentState is GameLevelConfig levelConfig)
            {
                switch (levelConfig.Time)
                {
                    case DayNight.Day:
                        currentGameState = new GameDayState(levelConfig.ScenePath);
                        break;
                    case DayNight.Night:
                        currentGameState = new GameNightState(levelConfig.ScenePath);
                        break;
                    default:
                        break;
                }
            }
            else if (gameconfig.CurrentState is CutsceneConfig cutsceneConfig)
            {
                currentGameState = new CutsceneState(cutsceneConfig.ScenePath);
            }

            currentGameState.SetConfig(gameconfig.CurrentState);

            tempStateDict.Add(gameconfig.CurrentState, currentGameState);
            gameStates.Add(currentGameState);

            stateMachine.RegisterStates(currentGameState);
            gameState.Add(currentGameState);
        }

        //Chain together
        for (int i = 0; i < gameStates.Count; i++)
        {
            if (i == 0)
            {
                continue;
            }

            if (i == gameStates.Count - 1)
            {
                //go back to menu
                var lastTransition = new SceneTransitionState(GameFlow.GameFlowSequences[i].CurrentState, GameFlow.GameFlowSequences[0].CurrentState);
                stateMachine.RegisterStates(lastTransition);
                stateMachine.Configure(gameStates[i]).AddTransition<StateChangeTrigger>(lastTransition);
                stateMachine.Configure(lastTransition).AddTransition<StateChangeTrigger>(gameStates[0]);
            }

            var transitionState = new SceneTransitionState(GameFlow.GameFlowSequences[i - 1].CurrentState, GameFlow.GameFlowSequences[i].CurrentState);
            stateMachine.RegisterStates(transitionState);
            stateMachine.Configure(gameStates[i - 1]).AddTransition<StateChangeTrigger>(transitionState);
            stateMachine.Configure(transitionState).AddTransition<StateChangeTrigger>(gameStates[i]);

            if (GameFlow.GameFlowSequences[i].FailState != null)
            {
                var transitionFailState = new SceneTransitionState(GameFlow.GameFlowSequences[i].CurrentState, GameFlow.GameFlowSequences[i].FailState);
                stateMachine.Configure(gameStates[i]).AddTransition<StateFailTrigger>(transitionFailState);
                stateMachine.Configure(transitionFailState).AddTransition<StateChangeTrigger>(tempStateDict[GameFlow.GameFlowSequences[i].FailState]);
            }
        }

        //default set to first state
        stateMachine.ChangeState(gameStates[0]);

        //LoadScene FirstScene
        SceneManager.LoadSceneAsync(GetScenePath(gameStates[0].GetConfig()), LoadSceneMode.Additive);
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

    public void PlayTutorial(string tutorialName)
    {
        if (!hasPlayedTutorials.Contains(tutorialName))
            hasPlayedTutorials.Add(tutorialName);
    }

    public bool HasPlayedTutorial(string tutorialName)
    {
        return hasPlayedTutorials.Contains(tutorialName);
    }

    public void OnWeb()
    {
        if (firstWeave)
        {
            firstWeave = false;

            Canvas.Find("GameUI/FirstWeavePanel").GetComponent<CongratsPanel>().ShowPanel();
        }
    }

    public void OnMeal()
    {
        if (firstMeal)
        {
            firstMeal = false;

            Canvas.Find("GameUI/FirstMealPanel").GetComponent<CongratsPanel>().ShowPanel();
        }
        FliesEatenThisLevel++;
    }

    public Transform BoardRoot;
    //after scene is loaded

    public IEnumerator WaitOneFrame(GameLevelConfig levelConfig)
    {
        yield return new WaitForEndOfFrame();
        CreateGame(levelConfig);
    }

    public void GameStartCoroutine(IEnumerator enumerator)
    {
        StartCoroutine(enumerator);
    }

    private GameLevelConfig CurrentConfig;

    public GameLevelConfig GetCurrentLevelConfig()
    {
        return CurrentConfig;
    }

    public void CreateGame(GameLevelConfig levelConfig)
    {
        Debug.Log($"========> CREATE GAME ! {levelConfig.ScenePath}");

        CurrentConfig = levelConfig;

        // 在这个场景里找出生点
        BoardRoot = GameObject.FindAnyObjectByType<Board>().transform;

        WeaveBoardManager.instance.CreatePlayground(levelConfig.SpawnBorder);

        //Generate Character
        var player = Instantiate(playerPrefab, levelConfig.PlayerPosition, Quaternion.identity, BoardRoot);

        this.player = player.GetComponent<SpiderController>();

        this.player.Init(levelConfig.PlayerStamina, levelConfig.PlayerAvailableNodes);

        if (levelConfig.CreateMother)
        {
            var mother = Instantiate(motherPrefab, levelConfig.MotherPosition, Quaternion.identity, BoardRoot);

            this.mother = mother.GetComponent<SpiderController>();

            this.mother.Init(levelConfig.MotherStamina, 0);
        }

        WeaveBoardManager.instance.RecoverPlayground();

        //lock nodes
        foreach(var lockPos in levelConfig.lockNodes)
        {
            WeaveBoardManager.instance.LockNode(lockPos);
        }

        if (levelConfig.PlayTutorial)
        {
            TutorialManager.Instance.StartTutorial();
        }

        if (levelConfig.HideSleepButton)
        {
            sleepButton.gameObject.SetActive(false);
        }
        else
        {
            sleepButton.gameObject.SetActive(true);
        }

        if (levelConfig.Time == DayNight.Day)
        {
            DayStatus.SetActive(true);
            NightStatus.SetActive(false);
        }

        if (levelConfig.Time == DayNight.Night)
        {
            DayStatus.SetActive(false);
            NightStatus.SetActive(true);
        }

        FliesEatenThisLevel = 0;
        GameManager.Instance.ResetTurn();

        GameObject.Find("BG").GetComponent<SpriteRenderer>().sprite = levelConfig.BackgroundSprite;
        FadeOut(() =>
        {

        });
    }

    private int FliesEatenThisLevel;

    public bool ReachQuota()
    {
        return FliesEatenThisLevel >= CurrentConfig.FlyQuota;
    }

    public int GetFliesEatenThisLevel()
    {
        return FliesEatenThisLevel;
    }

    public int GetNodesThisLevel()
    {
        return numberOfNodesAvailable;
    }

    // go to sleep
    public bool EndTurn()
    {
        if (TutorialManager.Instance.InTutorial)
            return false;

        if (ThisTurnHasEnd)
            return false;

        //check quota: on check during day time
        // 1. Fail: reset level
        // 2. Success: trigger

        if (currentTime == DayNight.Day)
        {
            //quota Check
            if (!ReachQuota())
            {
                //reset level to night time
                //pop the fail screen
                Canvas.Find("GameUI/FailPanel").GetComponent<FailPanel>().ShowPanel();
                return true;
            }
        }
        Debug.Log($"==========> TRIGGER CHANGE");
        stateMachine.Trigger<StateChangeTrigger>();
        return true;
    }
}

public enum DayNight
{
    Day = 0,
    Night = 1,
}