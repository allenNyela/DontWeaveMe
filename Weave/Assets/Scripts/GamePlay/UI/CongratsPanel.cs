using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CongratsPanel : MonoBehaviour
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

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnButtonClicked()
    {
        if (!canClick)
        {
            return;
        }
        this.gameObject.SetActive(false);
    }

    public void ShowPanel()
    {
        if (hasTrigger)
            return;
        hasTrigger = true;
        button.gameObject.SetActive(true);
        canvasGroup.DOFade(1, 0.5f).onComplete += () =>
        {
            canClick = true;
        };
    }
}
