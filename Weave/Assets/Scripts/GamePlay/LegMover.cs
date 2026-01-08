using UnityEngine;

public class LegMover : MonoBehaviour
{
    public Transform limbSolverTarget;
    public float moveDistance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(limbSolverTarget.position, transform.position) > moveDistance)
        {
            limbSolverTarget.position = transform.position;
        }
    }
}
