using UnityEngine;

public class CustomWebNode : WebNode
{
    public Vector2 customPosition;
    public bool onPlatform = false;


    public override void HandleOnTrigger(Collider2D other)
    {
        base.HandleOnTrigger(other);

        if (other.CompareTag("Platform"))
        {
            onPlatform = true;
        }
    }

    public override void HandleOnTriggerExit2D(Collider2D other)
    {
        base.HandleOnTriggerExit2D(other);

        if (other.CompareTag("Platform"))
        {
            onPlatform = false;
        }
    }

    public override void HandleOnTriggerStay2D(Collider2D other)
    {
        base.HandleOnTriggerStay2D(other);

        if (!onPlatform && other.CompareTag("Platform"))
        {
            onPlatform = true;
        }
    }

}


