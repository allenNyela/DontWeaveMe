using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;
    public bool InTutorial = false;
    public PlayableDirector tutorialTimeline;
    private bool hasFinished = false;
    public bool startTurial = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // start tutorial from script
        //if (startTurial)
        //{
        //    StartTutorial();
        //}
    }

    public void StartTutorial()
    {
        if (GameManager.Instance.HasPlayedTutorial(tutorialTimeline.playableAsset.name))
        {
            return;
        }

        //BindTrack

        foreach (var output in tutorialTimeline.playableAsset.outputs)
        {
            if (output.streamName == "MotherTrack") 
            {
                tutorialTimeline.SetGenericBinding(output.sourceObject, GameManager.Instance.mother.gameObject);
            }

            // 绑定 SignalTrack
            if (output.streamName == "MotherSignalTrack") // 轨道名字
            {
                tutorialTimeline.SetGenericBinding(output.sourceObject, GameManager.Instance.mother.gameObject.GetComponent<SignalReceiver>());
            }

            if (output.streamName == "PlayerSignalTrack") // 轨道名字
            {
                tutorialTimeline.SetGenericBinding(output.sourceObject, GameManager.Instance.player.gameObject.GetComponent<SignalReceiver>());
            }

        }

        tutorialTimeline?.Play();
        tutorialTimeline.stopped += OnTimelineFinish;
        InTutorial = true;
        GameManager.Instance.PlayTutorial(tutorialTimeline.playableAsset.name);
    }

    private void Awake()
    {
        Instance = this;
    }

    private void OnTimelineFinish(PlayableDirector d)
    {
        hasFinished = true;
        InTutorial = false;
    }

    public void PlayTutorial()
    {
        tutorialTimeline.Play();
        if (hasFinished)
            return;
    }

    public void StopTutorial()
    {
        tutorialTimeline.Pause();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}