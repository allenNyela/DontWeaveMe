using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WebNode : MonoBehaviour
{
    // pos
    public Vector2Int gridPos;

    private bool lockNode = false; //lock for player

    // connect nodes
    private List<WebNode> neighbors = new List<WebNode>();

    public void LockNode()
    {
        lockNode = true;
    }

    public void UnlockNode()
    {
        lockNode = false;
    }

    public int GetNeighborCount()
    {
        return neighbors.Count;
    }

    public void AddNeighnors(WebNode webNode)
    {
        if (!neighbors.Contains(webNode))
        {
            neighbors.Add(webNode);
            this.GetComponent<SpriteRenderer>().color = WeaveBoardManager.instance.connectColor;
        }

    }

    public void ClearNeighbors()
    {
        neighbors.Clear();
        this.GetComponent<SpriteRenderer>().color = WeaveBoardManager.instance.normalColor;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        var controller = other.gameObject.GetComponent<SpiderController>();

        if (controller == null)
            return;

        if (lockNode && controller.isPlayer)
            return;

        if (neighbors.Count >= 1 && (controller.currentChains.Count == 0))
            return;

        //only for closing
        if (neighbors.Count >= 1 && (controller.currentChains.Count > 0 && !controller.currentChains.Contains(this)))
            return;
        if (other.tag == "Player")
        {
            this.GetComponent<SpriteRenderer>().color = WeaveBoardManager.instance.highlightColor;

            other.gameObject.GetComponent<SpiderController>().highlightNode = this;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        var controller = other.gameObject.GetComponent<SpiderController>();

        if (controller == null)
            return;

        if (lockNode && controller.isPlayer)
            return;

        if (neighbors.Count >= 1 && (controller.currentChains.Count == 0))
            return;

        //only for closing
        if (neighbors.Count >= 1 && (controller.currentChains.Count > 0 && !controller.currentChains.Contains(this)))
            return;
        if (other.tag == "Player")
        {
            this.GetComponent<SpriteRenderer>().color = WeaveBoardManager.instance.highlightColor;

            other.gameObject.GetComponent<SpiderController>().highlightNode = this;
        }
    }


    private void OnTriggerExit2D(Collider2D other)
    {

        if (other.tag == "Player")
        {
            this.GetComponent<SpriteRenderer>().color = neighbors.Count > 0 ? WeaveBoardManager.instance.connectColor : WeaveBoardManager.instance.normalColor;

            other.gameObject.GetComponent<SpiderController>().highlightNode = null;
        }
    }
}
