using UnityEngine;

public class Interactable : MonoBehaviour
{
    public Sprite hintSprite;
    public GameObject spriteRenderer;
    public float scale = 10;
    public int order = 500;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        var spider = collision.gameObject.GetComponent<SpiderController>();
        if (spider != null && spider.isPlayer)
        {
            ShowHint();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var spider = collision.gameObject.GetComponent<SpiderController>();
        if (spider != null && spider.isPlayer)
        {
            HideHint();
        }
    }

    public void GenerateHint(Vector3 position)
    {
        if (spriteRenderer == null)
        {
            var go = new GameObject("Hint");
            go.transform.parent = this.transform;
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = new Vector3(scale, scale, scale);

            var spriteRender = go.AddComponent<SpriteRenderer>();
            spriteRender.sprite = hintSprite;
            spriteRender.sortingOrder = order;

            this.spriteRenderer = spriteRender.gameObject;

            go.transform.position = position;
        }
        spriteRenderer.gameObject.SetActive(false);
    }

    public void ShowHint()
    {
        if (spriteRenderer == null)
        {
            var go = new GameObject("Hint");
            go.transform.parent = this.transform;
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = new Vector3(scale, scale, scale);

            var spriteRender = go.AddComponent<SpriteRenderer>();
            spriteRender.sprite = hintSprite;
            spriteRender.sortingOrder = order;

            this.spriteRenderer = spriteRender.gameObject;
        }

        spriteRenderer.gameObject.SetActive(true);

    }

    public void HideHint()
    {
        spriteRenderer.gameObject.SetActive(false);
    }

}
