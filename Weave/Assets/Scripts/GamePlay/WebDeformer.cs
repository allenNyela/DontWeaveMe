using UnityEngine;
using System;

public class WebDeformer : MonoBehaviour
{
    [Serializable]
    public enum BindingKind
    {
        None,
        Node,
        EdgeSegment
    }


    [Serializable]
    public class VertexBinding
    {
        public BindingKind kind;
        public WebNode node;
        public WebEdge edge; 
        public int segmentIndex;  
    }

    public VertexBinding[] bindings;  // 和 mesh.vertices 一一对应

    Mesh _mesh;
    Transform _tf;

    void Awake()
    {
        var mf = GetComponent<MeshFilter>();
        if (mf != null)
        {
            // 一定要拿实例，不要改 sharedMesh
            _mesh = mf.mesh;
        }
        _tf = transform;
    }

    void LateUpdate()
    {
        if (_mesh == null || bindings == null) return;

        var verts = _mesh.vertices;

        int count = Mathf.Min(verts.Length, bindings.Length);
        for (int i = 0; i < count; i++)
        {
            var b = bindings[i];
            if (b == null || b.kind == BindingKind.None) continue;

            Vector3 worldPos;

            switch (b.kind)
            {
                case BindingKind.Node:
                    if (b.node == null) continue;
                    worldPos = b.node.transform.position;
                    break;

                case BindingKind.EdgeSegment:
                    if (b.edge == null) continue;
                    var segs = b.edge.segmentBodies;
                    if (segs == null || b.segmentIndex < 0 || b.segmentIndex >= segs.Count) continue;
                    worldPos = segs[b.segmentIndex].position;
                    break;

                default:
                    continue;
            }

            // mesh 里是 local 坐标
            verts[i] = _tf.InverseTransformPoint(worldPos);
        }

        _mesh.vertices = verts;
        _mesh.RecalculateBounds();
        // 2D 里法线不太重要，可以不每帧算 normals
    }
}
