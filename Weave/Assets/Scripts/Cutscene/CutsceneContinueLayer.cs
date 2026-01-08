using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using DG.Tweening;


public class CutsceneContinueLayer : MonoBehaviour
{
    public PlayableDirector cutsceneDirector;

    public CanvasGroup canvasGroup;

    public bool canClick = false;

    public bool hasTrigger = false;

    public Button clickButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        clickButton.onClick.RemoveAllListeners();

        clickButton.onClick.AddListener(OnClickButtonTrigger);

        cutsceneDirector.stopped += OnDirectorStop;
    }

    private void OnClickButtonTrigger()
    {
        if (!canClick)
            return;
        if (hasTrigger)
            return;

        hasTrigger = true;
        GameManager.Instance.stateMachine.Trigger<StateChangeTrigger>();
    }

    private void OnDirectorStop(PlayableDirector director)
    {
        canvasGroup.DOFade(1, 0.5f).onComplete += () =>
        {
            canClick = true;
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
