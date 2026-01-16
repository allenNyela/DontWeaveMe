using UnityEngine;
using TMPro;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif
using LibTessDotNet;
using System.Linq;
using DG.Tweening;

public class WeaveBoardManager : MonoBehaviour
{
    public int cellX;
    public int cellY;
    public float cellSpacing;
    public GameObject nodeSprite;
    public float padding = 0.5f;     // world-space padding inside the border

    [SerializeField]
    public WebNode[,] nodes;
    public static WeaveBoardManager instance;

    //node => color
    public Color normalColor = Color.white;
    public Color highlightColor = Color.yellow;
    public Color connectColor = Color.green;


    public Color loopFillColorMother = new Color(0.2f, 0.4f, 1f, 0.6f);
    public Color loopFillColorPlayer = new Color(0.9f, 0.8f, 0.2f, 0.6f);

    //player only for undo highlight
    public Color HighlightWebColor = Color.lightCyan;

    public GameObject edgeSegmentPrefab;
    public GameObject linePrefab;

    public List<Web> webs = new List<Web>();

    private readonly List<Transform> _vibratingSegments = new List<Transform>();

    public int segCounter = 0; //id purpose
    public int edgeCounter = 0; //id purpose

    public GameObject nodeObject;

    public Sprite webRedoSprite;

    public int silkWebOrder;

    public int defaultSilkWebOrder = 0;

    public GameObject flyPrefab;

    //store data purpose
    public struct WebStruct
    {
        public List<WebEdgeStruct> edges;
        public bool isPlayer;
    }

    //store data purpose
    public struct WebEdgeStruct
    {
        public NodeStruct nodeA;
        public NodeStruct nodeB;
    }

    public struct NodeStruct
    {
        public Vector2 pos;
        public bool isCustom;
    }

    private void Awake()
    {
        instance = this;
    }


    void Start()
    {

    }

    public void CreatePlayground(bool spawnBorder)
    {
        webs.Clear();
        nodes = new WebNode[cellX, cellY];
        ScanEnvironment(spawnBorder);
        //SpawnBorder();


    }

    public void RecoverPlayground()
    {
        if (GameManager.Instance._recover)
        {
            GameManager.Instance._recover = false;
            PerserveWebData(GameManager.Instance._perserveData);

            //spawn flies
            DeterminePlacement.Instance.SpawnObject();

            GameManager.Instance._perserveData = new List<WebStruct>();
        }

        silkWebOrder = defaultSilkWebOrder;
    }

    public void SpawnFly(Vector3 pos)
    {
        Instantiate(flyPrefab, pos, Quaternion.identity, GameManager.Instance.BoardRoot.transform);
    }

    private void ScanEnvironment(bool spawnBorder = true)
    {
        //delete recent and spawn based on border
        var webNodes = GameObject.FindObjectsByType<WebNode>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID);
        if (spawnBorder)
        {
            foreach (var node in webNodes)
            {
                GameObject.Destroy(node.gameObject);
                //this.nodes[node.gridPos.x, node.gridPos.y] = node;
            }
        }

        if (spawnBorder)
            SpawnBorder();
    }

    public void LockNode(Vector2Int pos)
    {
        if (nodes[pos.x, pos.y] != null)
        {
            nodes[pos.x, pos.y].LockNode();
        }
    }

    void SpawnBorder()
    {
        if (!nodeSprite)
        {
            Debug.LogError("nodeSprite not assigned.");
            return;
        }

        Camera cam = Camera.main;
        float halfH = cam.orthographicSize;
        float halfW = halfH * cam.aspect;

        float topY = halfH - padding;
        float bottomY = -halfH + padding;
        float leftX = -halfW + padding;
        float rightX = halfW - padding;

        int topRow = cellY - 1;
        int leftCol = 0;
        int rightCol = cellX - 1;

        // top
        for (int x = 0; x < cellX; x++)
        {
            float t = (cellX == 1) ? 0.5f : (float)x / (cellX - 1);
            float wx = Mathf.Lerp(leftX, rightX, t);

            Vector3 pos = new Vector3(wx, topY, 0f);
            GameObject go = Instantiate(nodeSprite, pos, Quaternion.identity, GameManager.Instance.BoardRoot);
            go.gameObject.SetActive(true);

            WebNode node = go.GetComponent<WebNode>();
            if (node == null) node = go.AddComponent<WebNode>();

            node.gridPos = new Vector2Int(x, topRow);
            nodes[x, topRow] = node;
        }

        // left and right
        for (int y = 1; y < cellY - 1; y++)
        {
            float t = (float)y / (cellY - 1);
            float wy = Mathf.Lerp(bottomY, topY, t);

            // left
            {
                Vector3 pos = new Vector3(leftX, wy, 0f);
                GameObject go = Instantiate(nodeSprite, pos, Quaternion.identity, GameManager.Instance.BoardRoot);
                go.gameObject.SetActive(true);
                WebNode node = go.GetComponent<WebNode>();
                if (node == null) node = go.AddComponent<WebNode>();

                node.gridPos = new Vector2Int(leftCol, y);
                nodes[leftCol, y] = node;
            }

            // right
            {
                Vector3 pos = new Vector3(rightX, wy, 0f);
                GameObject go = Instantiate(nodeSprite, pos, Quaternion.identity, GameManager.Instance.BoardRoot);
                go.gameObject.SetActive(true);
                WebNode node = go.GetComponent<WebNode>();
                if (node == null) node = go.AddComponent<WebNode>();

                node.gridPos = new Vector2Int(rightCol, y);
                nodes[rightCol, y] = node;
            }
        }
    }

    public void Connect(WebNode a, WebNode b, SpiderController spider, bool withCost = true)
    {
        if (a == null || b == null || a == b)
            return;

        var distance =  (int)(a.gridPos - b.gridPos).magnitude;
        if (spider.stamina >= distance)
        {
            a.AddNeighnors(b);
            b.AddNeighnors(a);
            if (withCost)
            {
                spider.currentChainStamina += distance;
                spider.ConsumeStamina(distance);
            }
        }
        else
        {
            Debug.Log($"============ NOT ENOUGH STAMINA");
        }
       
    }

    public Vector2Int WorldToGridPos(Vector3 worldPos)
    {
        Camera cam = Camera.main;
        float halfH = cam.orthographicSize;
        float halfW = halfH * cam.aspect;

        float topY = halfH - padding;
        float bottomY = -halfH + padding;
        float leftX = -halfW + padding;
        float rightX = halfW - padding;

        float tx = (worldPos.x - leftX) / (rightX - leftX);
        float ty = (worldPos.y - bottomY) / (topY - bottomY);

        int gx = Mathf.RoundToInt(tx * (cellX - 1));
        int gy = Mathf.RoundToInt(ty * (cellY - 1));

        gx = Mathf.Clamp(gx, 0, cellX - 1);
        gy = Mathf.Clamp(gy, 0, cellY - 1);

        return new Vector2Int(gx, gy);
    }

    public void RecoverBoard(List<WebStruct> data)
    {
        GameManager.Instance._recover = true;
        GameManager.Instance._perserveData = data;
    }

    private void PerserveWebData(List<WebStruct> data)
    {
        foreach(var webStruct in data)
        {
            var loopNodes = new List<WebNode>();

            var controller = webStruct.isPlayer ? GameManager.Instance.player : GameManager.Instance.mother;
            foreach (var edge in webStruct.edges)
            {
                var webNodeA = GetNode(edge.nodeA);
                var webNodeB = GetNode(edge.nodeB);

                controller.ConnectNodes(webNodeA, webNodeB);

                if (!loopNodes.Contains(webNodeA))
                {
                    loopNodes.Add(webNodeA);
                }
                if (!loopNodes.Contains(webNodeB))
                {
                    loopNodes.Add(webNodeB);
                }
            }
            CreateFillFromLoop(loopNodes, controller);
        }
    }

    public WebNode GetNode(NodeStruct nodeData)
    {
        if (!nodeData.isCustom)
            return nodes[(int)nodeData.pos[0], (int)nodeData.pos[1]];
        else
        {
            GameObject newNode = Instantiate(WeaveBoardManager.instance.nodeObject, nodeData.pos, Quaternion.identity, GameManager.Instance.BoardRoot.gameObject.transform);
            var customNode = newNode.GetComponent<CustomWebNode>();
            customNode.transform.position = nodeData.pos;
            customNode.gridPos = WeaveBoardManager.instance.WorldToGridPos(nodeData.pos);

            return customNode;
        }
    }

    public NodeStruct GetNodeStruct(WebNode web)
    {
        bool isCustom = web is CustomWebNode custom;
        if (isCustom)
        {
            return new NodeStruct()
            {
                pos = (web as CustomWebNode).transform.position,
                isCustom = true,
            };
        }
        else
        {
            return new NodeStruct()
            {
                pos = web.gridPos,
                isCustom = false,
            };
        }
    }

    public List<WebStruct> GenerateWebData()
    {
        List<WebStruct> webData = new List<WebStruct>();
        foreach (var web in webs)
        {
            WebStruct webStruct = new WebStruct();
            var edges = new List<WebEdgeStruct>();
            webStruct.isPlayer = web.owner.isPlayer;

            foreach(var edge in web.connectedEdges)
            {
                if (edge.nodeA != null && edge.nodeB != null)
                {
                    edges.Add(new WebEdgeStruct()
                    {
                        nodeA = GetNodeStruct(edge.nodeA),
                        nodeB = GetNodeStruct(edge.nodeB),
                    });
                }
            }
            webStruct.edges = edges;

            webData.Add(webStruct);
        }
        return webData;
    }

    public void OnPlayerConnectToNode(WebNode node, SpiderController spider)
    {
        if (node == null) return;

        if (spider.currentLoop.Count == 0)
        {
            spider.currentLoop.Add(node); 
            return;
        }

        if (spider.currentLoop[spider.currentLoop.Count - 1] == node)
            return;

        spider.currentLoop.Add(node);

        if (node != spider.currentLoop[0])
            return;

        if (spider.currentLoop.Count < 4) 
        {
            spider.currentLoop.Clear();
            spider.currentLoop.Add(node);
            return;
        }

        var loopNodes = spider.currentLoop.GetRange(0, spider.currentLoop.Count - 1);

        CreateFillFromLoop(loopNodes, spider);

        spider.currentLoop.Clear();
        spider.currentLoop.Add(node); 
    }

    public bool IsPositionWithinFly(Vector2 pos)
    {
        var ret = false;
        var allFlies = GameObject.FindObjectsOfType<Fly>();
        foreach(var fly in allFlies)
        {
            ret = fly.GetComponent<BoxCollider2D>().OverlapPoint(pos);
            if (ret)
            {
                return ret;
            }
        }
        return ret;
    }

    public bool IsPositionWithinWeb(Vector2 pos)
    {
        var ret = false;
        foreach(var web in webs)
        {
            ret = web.GetComponent<PolygonCollider2D>().OverlapPoint(pos);
            if (ret)
            {
                return ret;
            }
        }
        return ret;
    }

    void CreateFillFromLoop(List<WebNode> loopNodes, SpiderController spider)
    {
        if (loopNodes == null || loopNodes.Count < 3)
            return;

        // 1. 先构建一圈 contour 顶点 + 对应的 Binding
        var contour = new List<ContourVertex>();

        int n = loopNodes.Count;

        for (int i = 0; i < n; i++)
        {
            WebNode a = loopNodes[i];
            WebNode b = loopNodes[(i + 1) % n];

            // 找这两个 node 之间的 WebEdge（你要自己实现）
            WebEdge edge = FindEdgeBetween(a, b, spider); // 没有就 null

            // 第一个点：只有在 i == 0 时加一次 a（避免重复）
            if (i == 0)
            {
                contour.Add(MakeContourVertexForNode(a));
            }

            // 如果这条边有物理绳子，把 segment 也插进去
            if (edge != null && edge.segmentBodies != null && edge.segmentBodies.Count > 0)
            {
                bool fromA = EdgeStartsFrom(edge, a); // 根据你存 nodeA/nodeB 判断方向

                var segs = edge.segmentBodies;

                if (fromA)
                {
                    for (int s = 0; s < segs.Count; s++)
                        contour.Add(MakeContourVertexForSegment(edge, segs, s));
                }
                else
                {
                    for (int s = segs.Count - 1; s >= 0; s--)
                        contour.Add(MakeContourVertexForSegment(edge, segs, s));
                }
            }

            // 终点 b
            contour.Add(MakeContourVertexForNode(b));
        }

        // 2. 丢给 LibTess
        Tess tess = new Tess();
        tess.AddContour(contour.ToArray(), ContourOrientation.Original);
        tess.Tessellate(WindingRule.EvenOdd, ElementType.Polygons, 3);

        if (tess.ElementCount == 0)
        {
            Debug.Log("Tess result empty");
            return;
        }

        // 3. 根据 tess.Vertices 填 mesh 顶点和 binding
        var tVerts = tess.Vertices;
        Vector3[] verts = new Vector3[tVerts.Length];
        var bindings = new WebDeformer.VertexBinding[tVerts.Length];

        for (int i = 0; i < tVerts.Length; i++)
        {
            var tv = tVerts[i];
            verts[i] = new Vector3(tv.Position.X, tv.Position.Y, 0f);

            bindings[i] = tv.Data as WebDeformer.VertexBinding;
            // 内部新生成的点 Data 可能是 null，就保持 null → 这些顶点不会动，没关系
        }

        int[] tris = new int[tess.ElementCount * 3];
        int idx = 0;
        for (int i = 0; i < tess.ElementCount; i++)
        {
            int i0 = tess.Elements[i * 3 + 0];
            int i1 = tess.Elements[i * 3 + 1];
            int i2 = tess.Elements[i * 3 + 2];

            if (i0 == -1 || i1 == -1 || i2 == -1)
                continue;

            tris[idx++] = i0;
            tris[idx++] = i1;
            tris[idx++] = i2;
        }
        if (idx == 0) return;
        if (idx < tris.Length) System.Array.Resize(ref tris, idx);

        // 4. 创建 GameObject + Mesh + Deformer
        var go = new GameObject("LoopFill");
        go.transform.SetParent(GameManager.Instance.BoardRoot);
        go.transform.localPosition = Vector3.zero;

        var mf = go.AddComponent<MeshFilter>();
        var mr = go.AddComponent<MeshRenderer>();

        var mesh = new UnityEngine.Mesh();
        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mf.mesh = mesh;

        var poly = go.AddComponent<PolygonCollider2D>();

        Vector2[] colliderPoints = new Vector2[loopNodes.Count];
        for (int i = 0; i < loopNodes.Count; i++)
        {
            colliderPoints[i] = loopNodes[i].transform.position;
        }

        for (int i = 0; i < colliderPoints.Length; i++)
        {
            colliderPoints[i] = go.transform.InverseTransformPoint(colliderPoints[i]);
        }

        poly.points = colliderPoints;
        poly.isTrigger = true;

        //var mat = new Material(Shader.Find("Sprites/Default"));
        //mat.color = spider.isPlayer ? loopFillColorPlayer : loopFillColorMother;
        //mr.sharedMaterial = mat;

        var mat = new Material(Shader.Find("URP/SpiderWebCurved"));

        mat.SetColor("_BgColor", spider.isPlayer ? loopFillColorPlayer : loopFillColorMother);
        mat.SetColor("_LineColor", Color.white);

        // 根据你世界尺寸调这些
        mat.SetFloat("_RingCount", 20f);
        mat.SetFloat("_RingSpacing", 0.8f);
        mat.SetFloat("_RingThickness", 0.04f);
        mat.SetFloat("_RingSagAmount", 0.25f);   // 弧度（0~0.4 先试）

        mat.SetFloat("_RadialCount", 8f);
        mat.SetFloat("_RadialThickness", 0.04f);

        // 仍然是每个 loop 一个中心
        Vector3 c = ComputeLoopCenter(loopNodes);
        mat.SetVector("_WebCenter", new Vector4(c.x, c.y, c.z, 0));

        mr.sharedMaterial = mat;
        if (WeaveBoardManager.instance.silkWebOrder != WeaveBoardManager.instance.defaultSilkWebOrder)
        {
            //middle of highlighting
            Highlight.Instance.HighlightSingleMeshRenderer(mr);
        }

        var deformer = go.AddComponent<WebDeformer>();
        deformer.bindings = bindings;

        var web = go.AddComponent<Web>();
        web.SetOwner(spider);
        web.connectedNodes = loopNodes.ToList();
        web.connectedEdges = spider.currentLines.ToList();



        if (spider.isPlayer)
        {
            var hint = go.AddComponent<Interactable>();
            hint.hintSprite = webRedoSprite;
            hint.scale = 1;
            hint.GenerateHint(c);
        }

        spider.formingAWeb = true;
        if (spider.isPlayer)
        {
            GameManager.Instance.OnWeb();
        }

        webs.Add(web);
        UpdateVibration();
        spider.CancelWeave(true);
    }

    Vector3 ComputeLoopCenter(List<WebNode> loop)
    {
        if (loop == null || loop.Count == 0)
            return Vector3.zero;

        Vector3 sum = Vector3.zero;
        foreach (var n in loop)
            sum += n.transform.position;

        return sum / loop.Count;
    }


    ContourVertex MakeContourVertexForNode(WebNode node)
    {
        Vector3 p = node.transform.position;

        var cv = new ContourVertex();
        cv.Position = new Vec3(p.x, p.y, 0f);

        cv.Data = new WebDeformer.VertexBinding
        {
            kind = WebDeformer.BindingKind.Node,
            node = node,
            edge = null,
            segmentIndex = -1
        };

        return cv;
    }


    ContourVertex MakeContourVertexForSegment(WebEdge edge,
                                              IReadOnlyList<Rigidbody2D> segs,
                                              int segIndex)
    {
        Vector2 p = segs[segIndex].position;

        var cv = new ContourVertex();
        cv.Position = new Vec3(p.x, p.y, 0f);

        cv.Data = new WebDeformer.VertexBinding
        {
            kind = WebDeformer.BindingKind.EdgeSegment,
            node = null,
            edge = edge,
            segmentIndex = segIndex
        };

        return cv;
    }

    WebEdge FindEdgeBetween(WebNode a, WebNode b, SpiderController spider)
    {
        foreach (var e in spider.currentLines)
        {
            if ((e.nodeA == a && e.nodeB == b) ||
                (e.nodeA == b && e.nodeB == a))
                return e;
        }
        return null;
    }

    bool EdgeStartsFrom(WebEdge e, WebNode a)
    {
        return e.nodeA == a;
    }

    //reset level use
    public void ResetAllWeb()
    {
        foreach(var web in webs.ToArray())
        {
            web.UndoWeb();
            UndoWeb(web);
        }

    }

    //Refresh the whole board for vibration, remove the last viration and check again
    public void UpdateVibration()
    {
        var hotspots = new List<WeightedObject>();
        foreach(var wO in Object.FindObjectsByType<WeightedObject>(FindObjectsSortMode.InstanceID))
        {
            if (wO.type == AreaType.Hotspot)
            {
                hotspots.Add(wO);
            }
        }

        //clear vibration
        foreach (var t in _vibratingSegments)
        {
            if (t == null) continue;
            t.DOKill();  
        }
        _vibratingSegments.Clear();

        //checkrange
        foreach (var hotSpot in hotspots)
        {
            var circle = hotSpot.GetComponent<CircleCollider2D>();
            if (circle == null) continue;

            float radius = circle.radius * Mathf.Max(
                hotSpot.transform.lossyScale.x,
                hotSpot.transform.lossyScale.y);

            var pos = hotSpot.transform.position;
            var ret = GetClosestSegmentBody(pos, radius);
            foreach(var inRangeSeg in ret)
            {
                if (_vibratingSegments.Contains(inRangeSeg))
                    continue;

                _vibratingSegments.Add(inRangeSeg);

                //inRangeSeg.DOShakePosition(
                //    duration: 0.4f,
                //    strength: new Vector3(0.05f, 0.05f, 0f),
                //    vibrato: 10,
                //    randomness: 90,
                //    snapping: false,
                //    fadeOut: true
                //)
                //.SetLoops(-1, LoopType.Restart)
                //.SetUpdate(UpdateType.Fixed);
            }
        }
    }

    public void UndoWeb(Web web)
    {
        this.webs.Remove(web);
        UpdateVibration();
    }

    public List<Transform> GetClosestSegmentBody(Vector2 position, float radius)
    {
        List<Transform> ret = new List<Transform>();
        foreach (var web in webs)
        {
            foreach(var edge in web.connectedEdges)
            {
                foreach (var rb in edge.segmentBodies)
                {
                    if (rb == null) continue;

                    float d = (rb.position - position).magnitude;
                    if (d < radius)
                    {
                        ret.Add(rb.transform);
                    }
                }
            }

        }
        return ret;
    }

    [ContextMenu("Rebuild Border In Editor")]
    private void RebuildBorderInEditor()
    {
#if UNITY_EDITOR
        if (Application.isPlaying)
        {
            SpawnBorder();
            return;
        }

        if (nodes != null)
        {
            for (int x = 0; x < nodes.GetLength(0); x++)
            {
                for (int y = 0; y < nodes.GetLength(1); y++)
                {
                    if (nodes[x, y] != null)
                    {
                        DestroyImmediate(nodes[x, y].gameObject);
                    }
                }
            }
        }

        nodes = new WebNode[cellX, cellY];

        SpawnBorder();

        EditorSceneManager.MarkSceneDirty(gameObject.scene);
#endif
    }

}
