using UnityEngine;
using TMPro;

public class nodequotaUI : MonoBehaviour
{
    [SerializeField] private TMP_Text counterText;

    private void Awake()
    {
        if (counterText == null)
        {
            counterText = GetComponent<TMP_Text>();
        }
    }

    private void Update()
    {
        if (GameManager.Instance == null || counterText == null)
            return;

        int eaten = GameManager.Instance.player.numberOfNodesAvailable;
        int quota = GameManager.Instance.GetNodesThisLevel();
        //Debug.Log("Updating fly quota UI: " + eaten + "/" + quota);

        counterText.text = eaten + "/" + quota;
        // or: counterText.text = $"{eaten}/{quota}";
    }
}
