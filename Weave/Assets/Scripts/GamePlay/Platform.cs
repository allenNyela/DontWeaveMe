using UnityEditor;
using UnityEngine;

public class PlatformMove : MonoBehaviour
{
    public bool movingHorizontal = false;

    [SerializeField]
    public Vector2 leftBound = Vector2.left;
    [SerializeField]
    public Vector2 rightBound = Vector2.right;
    [HideInInspector]
    public Vector2 starterPosition;
    [HideInInspector]
    public Vector3 playerStartedPickUpPos;
    [HideInInspector]
    public bool playerinitPickup = false;

    // Left and right anchor points, define how far left and right the platform can move
    [HideInInspector]
    public Vector2 patrolLeftBound;
    [HideInInspector]
    public Vector2 patrolRightBound;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        patrolLeftBound = new Vector2(transform.localPosition.x, transform.localPosition.y) + leftBound;
        patrolRightBound = new Vector2(transform.localPosition.x, transform.localPosition.y) + rightBound;
        starterPosition = new Vector2(transform.position.x, transform.position.y);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region Editor Functions
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(leftBound.x, leftBound.y, 0));
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(rightBound.x, rightBound.y, 0));
    }
#endif
    #endregion
}


#if UNITY_EDITOR
[CustomEditor(typeof(PlatformMove))]
public class MinionHandles : Editor
{
    public void OnSceneGUI()
    {
        var linkedObject = target as PlatformMove;

        Handles.color = Color.white;

        Vector3 leftHandlePos = new Vector3(linkedObject.leftBound.x, linkedObject.leftBound.y, 0);
        Vector3 rightHandlePos = new Vector3(linkedObject.rightBound.x, linkedObject.rightBound.y, 0);

        Handles.DrawSolidDisc(linkedObject.transform.position + leftHandlePos, Vector3.forward, .1f);
        Handles.DrawSolidDisc(linkedObject.transform.position + rightHandlePos, Vector3.forward, .1f);

        EditorGUI.BeginChangeCheck();
        Vector3 updatedLeftPosition = Handles.PositionHandle(linkedObject.transform.position + leftHandlePos, Quaternion.identity);
        Vector3 updatedRightPosition = Handles.PositionHandle(linkedObject.transform.position + rightHandlePos, Quaternion.identity);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(target, "Update handle positions");
            linkedObject.leftBound = new Vector2(updatedLeftPosition.x - linkedObject.transform.position.x, 0);
            linkedObject.rightBound = new Vector2(updatedRightPosition.x - linkedObject.transform.position.x, 0);
        }
    }
}
#endif
