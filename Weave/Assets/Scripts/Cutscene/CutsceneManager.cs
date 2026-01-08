using UnityEngine;
using UnityEngine.Playables;

public class CutsceneManager : MonoBehaviour
{
    public PlayableDirector director;
    private TimelineWaitType currentWaitType;
    public static CutsceneManager Instance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PauseTimelineAndCheck(TimelineWaitType checkType)
    {
        currentWaitType = checkType;
        director.Pause();
        if (checkType == TimelineWaitType.Dialogue)
        {
            BubbleManager.Instance.PlayDialogue();
            AudioManager.Instance.getRandomVOFromList();
        }
    }

    public void PlayDirector(TimelineWaitType check)
    {
        if (check == currentWaitType)
        {
            currentWaitType = TimelineWaitType.None;
            director.Play();
        }
    }
}

public enum TimelineWaitType
{
    None,
    Custom,
    Dialogue,
}
