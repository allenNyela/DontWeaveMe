using DG.Tweening;
using TMPro;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    public TMP_Text text;
    public bool hasShow = false;
    public bool playing = false;

    public DOTweenAnimation bubbleAnimation;
    public DOTweenAnimation textAnimation;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.transform.localScale = Vector3.zero;
        text.alpha = 0;
    }

    public void ShowText()
    {
        if (hasShow)
            return;
        hasShow = true;
        playing = true;
        bubbleAnimation.onComplete.AddListener(() =>
        {
            textAnimation.DOPlay();
        });
        textAnimation.onComplete.AddListener(() =>
        {
            playing = false;
            OnTextShowEnd();
        });
        bubbleAnimation.DOPlay();
    }

    private void OnTextShowEnd()
    {
        //open any key canvas;
        BubbleManager.Instance.ShowCanvas();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
