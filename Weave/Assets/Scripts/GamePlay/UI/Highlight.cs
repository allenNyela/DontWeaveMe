using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Highlight : MonoBehaviour
{
    public static Highlight Instance;

    public int highLightFadeLayer = 950;

    public int highLightLayer = 1000;

    public Dictionary<RectTransform, int> previousLayers = new Dictionary<RectTransform, int>();

    public RectTransform maskLayer;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        maskLayer.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private RectTransform highlightingUI;

    private void HighlightUI(RectTransform itemToHighlight)
    {
        if (previousLayers.ContainsKey(itemToHighlight))
        {
            previousLayers.Remove(itemToHighlight);
        }

        if (itemToHighlight.GetComponent<Canvas>() != null)
        {
            previousLayers.Add(itemToHighlight, itemToHighlight.GetComponent<Canvas>().sortingOrder);
        }
        else
        {
            previousLayers.Add(itemToHighlight, -1);
        }
        highlightingUI = itemToHighlight;
        var canvas = itemToHighlight.gameObject.AddComponentIfNotExist<Canvas>();
        canvas.overrideSorting = true;
        canvas.sortingOrder = highLightLayer;
        maskLayer.gameObject.SetActive(true);
    }

    private void UnhighlightUI(RectTransform itemToHighlight)
    {
        maskLayer.gameObject.SetActive(false);

        if (previousLayers[itemToHighlight] != -1)
        {
            highlightingUI.GetComponent<Canvas>().sortingOrder = previousLayers[itemToHighlight];
        }
        else
        {
            Destroy(highlightingUI.GetComponent<Canvas>());
        }

        previousLayers.Remove(itemToHighlight);
        
        highlightingUI = null;
    }

    //code here to extend the function
    public void HighlightByType(HighlightEnum e)
    {
        switch (e)
        {
            case HighlightEnum.Node:
                HighlightSpriteRenderer(GetAllNodeSpriteRenderer());
                break;
            case HighlightEnum.PlayerBar:
                HighlightSpriteRenderer(GameManager.Instance.player.energyBar.GetSpriteRenderers());
                break;
            case HighlightEnum.MotherBar:
                HighlightSpriteRenderer(GameManager.Instance.mother.energyBar.GetSpriteRenderers());
                break;
            case HighlightEnum.Player:
                HighlightSpriteRenderer(GetPlayerSpriteRenderer());
                break;
            case HighlightEnum.Mother:
                HighlightSpriteRenderer(GetMotherSpriteRenderer());
                break;
            case HighlightEnum.Silk:
                WeaveBoardManager.instance.silkWebOrder = highlightSortorder;
                foreach (var web in GetAllWeb())
                {
                    HighlightSingleMeshRenderer(web);
                }

                foreach (var silk in GetAllSilk())
                {
                    HighlightSingleLineRenderer(silk);
                }
                break;
            default:
                break;
        }
    }

    public void HighlightByType(List<HighlightEnum> e)
    {
        foreach(var ele in e)
        {
            HighlightByType(ele);
        }
    }

    public void UnhighlightByType(List<HighlightEnum> e)
    {
        foreach(var ele in e)
        {
            UnhighlightByType(ele);
        }
    }

    public void UnhighlightByType(HighlightEnum e)
    {
        switch (e)
        {
            case HighlightEnum.Node:
                UnHighlightSpriteRenderer(GetAllNodeSpriteRenderer());
                break;
            case HighlightEnum.PlayerBar:
                UnHighlightSpriteRenderer(GameManager.Instance.player.energyBar.GetSpriteRenderers());
                break;
            case HighlightEnum.MotherBar:
                UnHighlightSpriteRenderer(GameManager.Instance.mother.energyBar.GetSpriteRenderers());
                break;
            case HighlightEnum.Player:
                UnHighlightSpriteRenderer(GetPlayerSpriteRenderer());
                break;
            case HighlightEnum.Mother:
                UnHighlightSpriteRenderer(GetMotherSpriteRenderer());
                break;
            case HighlightEnum.Silk:
                WeaveBoardManager.instance.silkWebOrder = 0;
                foreach(var web in GetAllWeb())
                {
                    UnHightlightMeshRenderer(web);
                }

                foreach(var silk in GetAllSilk())
                {
                    UnHightlightLineRenderer(silk);
                }
                //hightlight silk


                break;
            default:
                break;
        }
    }

    private List<LineRenderer> GetAllSilk()
    {
        var ret = GameObject.FindObjectsOfType<LineRenderer>(false);
        return ret.ToList();
    }

    private List<MeshRenderer> GetAllWeb()
    {
        var webs = GameObject.FindObjectsOfType<Web>(false);
        var ret = new List<MeshRenderer>();

        foreach(var web in webs)
        {
            ret.Add(web.gameObject.GetComponent<MeshRenderer>());
        }
        return ret;
    }

    private List<SpriteRenderer> GetAllNodeSpriteRenderer()
    {
        var allNode = GameObject.FindObjectsOfType<WebNode>(false);
        var ret = new List<SpriteRenderer>();
        foreach(var node in allNode)
        {
            ret.Add(node.GetComponent<SpriteRenderer>());
        }
        return ret;
    }

    private List<SpriteRenderer> GetPlayerSpriteRenderer()
    {
        var player = GameManager.Instance.player.visual.GetComponentsInChildren<SpriteRenderer>();
        return player.ToList();
    }

    private List<SpriteRenderer> GetMotherSpriteRenderer()
    {
        var player = GameManager.Instance.mother.visual.GetComponentsInChildren<SpriteRenderer>();
        return player.ToList();
    }



    //private List<SpriteRenderer> toHighlight;
    private Dictionary<SpriteRenderer, int> previousOrder = new Dictionary<SpriteRenderer, int>();
    private Dictionary<MeshRenderer, int> previousMeshOrder = new Dictionary<MeshRenderer, int>();
    private Dictionary<LineRenderer, int> previousLineOrder = new Dictionary<LineRenderer, int>();

    //private int previousOrder = -1;
    private int highlightSortorder = 5000;
    private int maskOrder = 4999;
    private SpriteRenderer overlay;
    private void HighlightSpriteRenderer(List<SpriteRenderer> toHighlight)
    {
        foreach(var t in toHighlight)
        {
            HighlightSingleSpriteRenderer(t);
        }
    }

    public void HighlightSingleMeshRenderer(MeshRenderer mr)
    {
        if (previousMeshOrder.ContainsKey(mr))
        {
            previousMeshOrder.Remove(mr);
        }

        previousMeshOrder.Add(mr, mr.sortingOrder);
        mr.sortingOrder = highlightSortorder;

        if (overlay == null)
        {
            overlay = CreateScreenOverlay(mr.transform.parent);
            GameManager.Instance.TurnOffGameStatsUI();
        }
    }

    public void HighlightSingleLineRenderer(LineRenderer lr)
    {
        if (previousLineOrder.ContainsKey(lr))
        {
            previousLineOrder.Remove(lr);
        }

        previousLineOrder.Add(lr, lr.sortingOrder);
        lr.sortingOrder = highlightSortorder;

        if (overlay == null)
        {
            overlay = CreateScreenOverlay(lr.transform.parent);
            GameManager.Instance.TurnOffGameStatsUI();
        }
    }

    private void HighlightSingleSpriteRenderer(SpriteRenderer sp)
    {
        if (previousOrder.ContainsKey(sp))
        {
            previousOrder.Remove(sp);
        }

        previousOrder.Add(sp, sp.sortingOrder);
        sp.sortingOrder = highlightSortorder + sp.sortingOrder;

        if (overlay == null)
        {
            overlay = CreateScreenOverlay(sp.transform.parent);
            GameManager.Instance.TurnOffGameStatsUI();
        }
    }

    private SpriteRenderer CreateScreenOverlay(Transform parent, float alpha = 0.98f)
    {
        var go = new GameObject("ScreenOverlay");
        var sr = go.AddComponent<SpriteRenderer>();
        go.transform.parent = parent;

        sr.sprite = GetWhiteSprite();
        sr.color = new Color(0, 0, 0, alpha);
        sr.sortingOrder = maskOrder;

        var cam = Camera.main;
        float height = 2f * cam.orthographicSize * 2000f;
        float width = height * cam.aspect * 2000f;

        go.transform.localScale = new Vector3(width, height, 1);

        go.transform.position = cam.transform.position + cam.transform.forward * 1f;

        return sr;
    }

    private static Sprite _whiteSprite;

    public static Sprite GetWhiteSprite()
    {
        if (_whiteSprite != null)
            return _whiteSprite;

        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();

        _whiteSprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
        return _whiteSprite;
    }

    private void UnHighlightSpriteRenderer(List<SpriteRenderer> toHightlights)
    {
        //recover game UI

        foreach(var h in toHightlights)
        {
            h.sortingOrder = previousOrder[h];
            previousOrder.Remove(h);
        }

        if (previousOrder.Count == 0 && previousMeshOrder.Count == 0 && previousLineOrder.Count == 0)
        {

            GameObject.Destroy(overlay.gameObject);

            GameManager.Instance.TurnOnGameStatsUI();

            overlay = null;
        }

    }

    public void UnHightlightMeshRenderer(MeshRenderer mr)
    {
        if (previousMeshOrder.ContainsKey(mr))
        {
            mr.sortingOrder = previousMeshOrder[mr];
            previousMeshOrder.Remove(mr);
        }

        if (previousOrder.Count == 0 && previousMeshOrder.Count == 0 && previousLineOrder.Count == 0)
        {

            GameObject.Destroy(overlay.gameObject);

            GameManager.Instance.TurnOnGameStatsUI();

            overlay = null;
        }

    }

    public void UnHightlightLineRenderer(LineRenderer lr)
    {
        if (lr == null)
            return;
        if (previousLineOrder.ContainsKey(lr))
        {
            lr.sortingOrder = previousLineOrder[lr];
            previousLineOrder.Remove(lr);
        }


        if (previousOrder.Count == 0 && previousMeshOrder.Count == 0 && previousLineOrder.Count == 0)
        {

            GameObject.Destroy(overlay.gameObject);

            GameManager.Instance.TurnOnGameStatsUI();

            overlay = null;
        }

    }
}

public enum HighlightEnum
{
    Node,
    MotherBar,
    PlayerBar,
    Player,
    Mother,
    Silk,
}
