using System.Collections.Generic;
using UnityEngine;

public class BubbleManager : MonoBehaviour
{
    public static BubbleManager Instance;
    public int currentIndex = 0;
    [SerializeField]
    public List<Bubble> Bubbles = new List<Bubble>();
    public BubbleContinueCanvas bubbleCanvas;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    public void PlayDialogue()
    {
        if (currentIndex > Bubbles.Count - 1)
        {
            Debug.Log($"WRONG NUMBERS OF BUBBLES, CHECK TIMELINE AND BUBBLEMANAGER");
            return;
        }
        Bubbles[currentIndex].ShowText();
        currentIndex++;
    }

    public void ShowCanvas()
    {
        bubbleCanvas.ShowButton();
    }

    private void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
