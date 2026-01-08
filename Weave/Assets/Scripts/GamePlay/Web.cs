using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Web : MonoBehaviour
{
    public SpiderController owner;
    public List<WebNode> connectedNodes;
    public List<WebEdge> connectedEdges;
    //energy that used to create this web
    public int energyCost => connectedEdges.Sum(x => x.energyCost);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetOwner(SpiderController s)
    {
        owner = s;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var spider = collision.gameObject.GetComponent<SpiderController>();
        if (spider != null && spider.isPlayer && owner.isPlayer)
        {
            Debug.Log($"Player Spider Entered");
            HightlightWeb();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var spider = collision.gameObject.GetComponent<SpiderController>();
        if (spider != null && spider.isPlayer && owner.isPlayer)
        {
            Debug.Log($"Player Spider Exited");
            UnHightlightWeb();
        }
    }

    private void HightlightWeb()
    {
        var mr = this.gameObject.GetComponent<MeshRenderer>();

        var mat = mr.material;
        mat.SetColor("_BgColor", WeaveBoardManager.instance.highlightColor);

        owner.highlightedWeb = this;
    }

    private void UnHightlightWeb()
    {
        var mr = this.gameObject.GetComponent<MeshRenderer>();
        owner.highlightedWeb = null;

        var mat = mr.material;
        mat.SetColor("_BgColor", WeaveBoardManager.instance.loopFillColorPlayer);
    }

    public void UndoWeb()
    {
        this.owner.GetStamina((int)((float)energyCost * 0.8f));
        foreach(var node in connectedNodes)
        {
            node.ClearNeighbors();
        }

        foreach(var edge in connectedEdges)
        {
            if (WeaveBoardManager.instance.silkWebOrder != WeaveBoardManager.instance.defaultSilkWebOrder)
            {
                Highlight.Instance.UnHightlightLineRenderer(edge.line);
            }
            GameObject.Destroy(edge.gameObject);
        }
        if (WeaveBoardManager.instance.silkWebOrder != WeaveBoardManager.instance.defaultSilkWebOrder)
        {
            Highlight.Instance.UnHightlightMeshRenderer(this.gameObject.GetComponent<MeshRenderer>());
        }
        GameObject.Destroy(this.gameObject);
    }

}
