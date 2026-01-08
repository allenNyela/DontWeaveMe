using System.Collections.Generic;
using UnityEngine;

public class WebEdge : MonoBehaviour
{
    public WebNode nodeA;
    public WebNode nodeB;
    public SpiderController owner;
    public int energyCost;
    public LineRenderer line;

    public int segmentCount = 5;
    public float springFrequency = 100f;
    public float springDamping = 0.9f;   
    public float breakForce = Mathf.Infinity;

    private Rigidbody2D startBody;
    private Rigidbody2D endBody;
    public readonly List<Rigidbody2D> segmentBodies = new List<Rigidbody2D>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        if (line == null || startBody == null || endBody == null)
            return;

        float maxSqrSpeed = 0f;
        foreach (var rb in segmentBodies)
            maxSqrSpeed = Mathf.Max(maxSqrSpeed, rb.linearVelocity.sqrMagnitude);

        // 速度很小就强制睡眠，防止慢慢抖
        if (maxSqrSpeed < 0.01f)
        {
            foreach (var rb in segmentBodies)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }
        }

        int idx = 0;
        line.SetPosition(idx++, startBody.position);

        for (int i = 0; i < segmentBodies.Count; i++)
        {
            line.SetPosition(idx++, segmentBodies[i].position);
        }

        line.SetPosition(idx, endBody.position);
    }

    public void GenerateWebPhysics()
    {
        startBody = GetOrCreateAnchorBody(nodeA);
        endBody = GetOrCreateAnchorBody(nodeB);

        Vector2 startPos = nodeA.transform.position;
        Vector2 endPos = nodeB.transform.position;

        int totalSegments = segmentCount;
        Vector2 step = (endPos - startPos) / (totalSegments + 1);

        Rigidbody2D prevBody = startBody;

        for (int i = 0; i < totalSegments; i++)
        {
            Vector2 pos = startPos + step * (i + 1);

            var segGO = Instantiate(WeaveBoardManager.instance.edgeSegmentPrefab, pos, Quaternion.identity, transform);
            segGO.name = $"WebSeg_{WeaveBoardManager.instance.segCounter++}";
            segGO.gameObject.SetActive(true);

            var rb = segGO.GetComponent<Rigidbody2D>();
            if (rb == null)
                rb = segGO.AddComponent<Rigidbody2D>();

            // 加入 segment list
            segmentBodies.Add(rb);

            // ---------- 绳子 joint（DistanceJoint2D） ----------
            var joint = segGO.AddComponent<DistanceJoint2D>();
            joint.connectedBody = prevBody;
            joint.autoConfigureDistance = false;

            float rest = Vector2.Distance(rb.position, prevBody.position) * 0.95f;
            joint.distance = rest;

            joint.maxDistanceOnly = false;
            joint.enableCollision = false;

            // 绳子阻尼（防止抖）
            rb.linearDamping = 1.8f;
            rb.angularDamping = 1.8f;
            rb.gravityScale = 0.7f;

            prevBody = rb;
        }

        // ---------- 最后一个 joint ----------
        var endJoint = endBody.gameObject.AddComponent<DistanceJoint2D>();
        endJoint.connectedBody = prevBody;
        endJoint.autoConfigureDistance = false;

        float finalRest = Vector2.Distance(endBody.position, prevBody.position) * 0.95f;
        endJoint.distance = finalRest;

        endJoint.maxDistanceOnly = false;
        endJoint.enableCollision = false;

        // 设置 linerender 点数
        line.positionCount = 2 + segmentBodies.Count;
    }


    Rigidbody2D GetOrCreateAnchorBody(WebNode node)
    {
        var t = node.transform;
        var rb = t.GetComponent<Rigidbody2D>();
        if (rb == null)
            rb = t.gameObject.AddComponent<Rigidbody2D>();

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        return rb;
    }
}
