using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;

public class EndLayer : MonoBehaviour
{
    public static EndLayer instance;

    public CanvasGroup canvas;

    public UnityEvent onFinishClick;

    public Button endButton;

    public bool canClick = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        endButton.onClick.RemoveAllListeners();
        endButton.onClick.AddListener(OnEndButtonClicked);

        endButton.gameObject.SetActive(false);
    }

    private void OnEndButtonClicked()
    {
        if (!canClick)
            return;

        onFinishClick?.Invoke();
        canClick = false;
    }

    //call this to end the level
    public void StartEnd()
    {
        endButton.gameObject.SetActive(true);
        canvas.DOFade(1, 0.5f).onComplete += () =>
        {
            canClick = true;
        };
    }

    private void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
