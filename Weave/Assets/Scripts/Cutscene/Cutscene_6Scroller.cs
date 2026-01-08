using UnityEngine;

public class Cutscene_6Scroller : MonoBehaviour
{
    public GameObject backgroundGO;
    public SpriteRenderer backgroundSR;
    public SpriteRenderer blackOverlaySR; // Assign in inspector: a child with a black sprite covering the screen
    public Sprite sprite1;
    public Sprite sprite2;
    public Sprite sprite3;
    public Sprite sprite4;
    private bool isFlashing = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isFlashing)
        {
            StartCoroutine(FlashAndChangeImage());
        }
    }

    private System.Collections.IEnumerator FlashAndChangeImage()
    {
        isFlashing = true;
        if (blackOverlaySR != null)
            blackOverlaySR.enabled = true;
        yield return new WaitForSeconds(1f);
        if (blackOverlaySR != null)
            blackOverlaySR.enabled = false;

        if (backgroundSR.sprite == sprite1)
        {
            backgroundSR.sprite = sprite2;
        }
        else if (backgroundSR.sprite == sprite2)
        {
            backgroundSR.sprite = sprite3;
        }
        else if (backgroundSR.sprite == sprite3)
        {
            backgroundSR.sprite = sprite4;
        }
        else if (backgroundSR.sprite == sprite4)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Cutscene_7");
        }
        isFlashing = false;
    }
}
