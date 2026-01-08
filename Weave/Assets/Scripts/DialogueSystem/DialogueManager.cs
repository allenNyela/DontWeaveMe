using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using static DialogueData;
using System.Text.RegularExpressions;

public class DialogueManager : MonoBehaviour
{
    [SerializeField]
    public DialogueDatabase database;

    private bool playing = false;

    private DialogueData currentDialogueData;

    private int currentIndex; //for current dialogue data

    private bool linePlaying = false;

    private bool waitingForResponse = false;

    public ConditionCheck conditionCheck;

    private bool conditionMet = false;

    private bool checkingCondition;

    public static DialogueManager Instance;

    public float charsPerSecond = 20f;

    static readonly Regex richTextTagRegex = new Regex("<.*?>", RegexOptions.Singleline);

    public float coolDownSkip = 1f;
    public float currentDt = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StopDialogue();
        database.Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && currentDialogueData != null)
        {
            if (linePlaying && currentDt >= coolDownSkip)
            {
                currentDt = 0f;
                seq.Complete(true);
            }
            if (!waitingForResponse)
                return;

            if (currentDt >= coolDownSkip)
                PlayNext();
        }
        currentDt += Time.deltaTime;
        CheckCondition();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        //DontDestroyOnLoad(gameObject);
    }

    public void StopDialogue()
    {
        GameManager.Instance.dialogue.gameObject.SetActive(false);
    }

    public void StartDialogue(int id)
    {
        if (playing)
            return;

        
        GameManager.Instance.dialogue.gameObject.SetActive(true);
        GameManager.Instance.dialogue.GetComponent<CanvasGroup>().alpha = 0f;
        GameManager.Instance.dialogue.GetComponent<CanvasGroup>().DOFade(1, 0.5f);

        currentIndex = 0;

        if (!database.DataBaseForDialogue.ContainsKey(id))
        {
            Debug.LogError($"Dialogue Data Not Found");
            return;
        }

        currentDialogueData = database.DataBaseForDialogue[id];

        PlayDialogue();
    }

    Sequence seq; //playing seq

    public void PlayDialogue()
    {
        AudioManager.Instance.getRandomVOFromList();
        TutorialManager.Instance.StopTutorial();
        checkingCondition = false;
        int characterId = currentDialogueData.CharacterId;
        string currentText = currentDialogueData.Dialogues[currentIndex].line;
        conditionCheck = currentDialogueData.Dialogues[currentIndex].conditionCheck;
        if (conditionCheck != ConditionCheck.None)
        {
            conditionMet = false;
        }
        else
        {
            conditionMet = true;
        }

        if (conditionCheck == ConditionCheck.ConnectNodeCheck)
        {
            startNode = GameManager.Instance.player.currentConnectingNode;
        }
        linePlaying = true;
        GameManager.Instance.player.inControl = false;
        seq = DOTween.Sequence();
        GameManager.Instance.dialogueText.text = "";
        GameManager.Instance.pointer.gameObject.SetActive(false);
        waitingForResponse = false;
        GameManager.Instance.player.inControl = false;

        string plain = richTextTagRegex.Replace(currentText, "");
        int charCount = plain.Length;
        float duration = charCount / charsPerSecond;

        seq.Append(GameManager.Instance.dialogueText.DOText(currentText, duration).SetEase(Ease.Linear));
        seq.AppendCallback(() =>
        {
            GameManager.Instance.player.inControl = true;
            waitingForResponse = true;
            linePlaying = false;
            GameManager.Instance.player.inControl = true;
            GameManager.Instance.pointer.gameObject.SetActive(true);

            if (conditionCheck != ConditionCheck.None)
            {
                checkingCondition = true;
            }
        });
    }

    private float wasdTime;
    private const float wasdTimeCheck = 3f;
    private WebNode startNode;
    private void CheckCondition()
    {
        if (checkingCondition)
        {
            switch (conditionCheck)
            {
                case ConditionCheck.WASD_Check:
                    if (GameManager.Instance.player.input.magnitude > 0)
                    {
                        wasdTime += Time.deltaTime;

                        if (wasdTime >= wasdTimeCheck)
                        {
                            conditionMet = true;
                        }

                        if (waitingForResponse && conditionMet)
                        {
                            PlayNext();
                            checkingCondition = false;
                        }
                    }
                    break;
                case ConditionCheck.ConnectNodeCheck:

                    if (GameManager.Instance.player.currentConnectingNode != null && startNode != GameManager.Instance.player.currentConnectingNode)
                    {
                        conditionMet = true;
                    }

                    if (waitingForResponse && conditionMet)
                    {
                        PlayNext();
                        checkingCondition = false;
                    }

                    break;
                case ConditionCheck.FormWeb:

                    if (GameManager.Instance.player.formingAWeb)
                    {
                        conditionMet = true;
                    }

                    if (waitingForResponse && conditionMet)
                    {
                        PlayNext();
                        checkingCondition = false;
                    }
                    break;

                case ConditionCheck.CancelNode:

                    if (GameManager.Instance.player.cancelNode)
                    {
                        conditionMet = true;
                    }

                    if (waitingForResponse && conditionMet)
                    {
                        PlayNext();
                        checkingCondition = false;
                    }
                    break;
            }
        }
    }

    public void PlayNext()
    {
        if (!waitingForResponse)
            return;
        if (!conditionMet)
            return;
        currentIndex++;
        if (currentIndex >= currentDialogueData.Dialogues.Count)
        {
            currentDialogueData = null;
            TutorialManager.Instance.PlayTutorial();
            //END;
            GameManager.Instance.dialogue.GetComponent<CanvasGroup>().alpha = 1f;
            GameManager.Instance.dialogue.GetComponent<CanvasGroup>().DOFade(0, 0.5f);
        }
        else
        {
            PlayDialogue();
        }
    }


}
