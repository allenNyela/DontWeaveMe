using UnityEngine;

public class CustomWebNode : WebNode
{
    public Vector2 customPosition;
    public bool onPlatform = false;

    void OnTriggerEnter2D(Collider2D interactable)
    {
        if (interactable.CompareTag("Platform"))
        {
            onPlatform = true;
        }
    }

    private void OnTriggerExit2D(Collider2D interactable)
    {
        if (interactable.CompareTag("Platform"))
        {
            onPlatform = false;
        }
    }

    void OnTriggerStay2D(Collider2D interactable)
    {
        if (!onPlatform && interactable.CompareTag("Platform"))
        {
            onPlatform = true;
        }
    }
}


