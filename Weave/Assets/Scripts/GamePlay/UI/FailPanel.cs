using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FailPanel : MonoBehaviour
{
    public bool hasTrigger = false;
    public bool canClick = false;

    public CanvasGroup canvasGroup;
    public Button button;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        button.gameObject.SetActive(false);
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnButtonClicked);

        canvasGroup.alpha = 0f;
    }

    private void OnButtonClicked()
    {
        if (!canClick)
        {
            return;
        }

        GameManager.Instance.stateMachine.Trigger<StateFailTrigger>();
        Debug.Log($"==========> TRIGGER Fail");
        canvasGroup.DOFade(0, 0.5f);

        canClick = false;
    }

    public void ResetTrigger()
    {
        hasTrigger = false;
    }

    public void ShowPanel()
    {
        if (hasTrigger)
            return;
        hasTrigger = true;
        canClick = false;
        button.gameObject.SetActive(true);
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
