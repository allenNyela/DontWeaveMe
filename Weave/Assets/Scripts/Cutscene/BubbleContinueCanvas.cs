using UnityEngine;
using UnityEngine.UI;

public class BubbleContinueCanvas : MonoBehaviour
{
    public Button clickButton;
    public bool canClick = false;
    void Start()
    {
        clickButton.onClick.RemoveAllListeners();

        clickButton.onClick.AddListener(OnClickButtonTrigger);

        clickButton.gameObject.SetActive(false);
    }

    public void ShowButton()
    {
        canClick = true;
        clickButton.gameObject.SetActive(true);
    }

    private void OnClickButtonTrigger()
    {
        if (!canClick)
            return;
        canClick = false;
        clickButton.gameObject.SetActive(false);
        CutsceneManager.Instance.PlayDirector(TimelineWaitType.Dialogue);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
