using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[CustomEditor(typeof(DeterminePlacement))]
public class SpawnComponentEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // Draws the default fields

        //DeterminePlacement spawnComponent = (DeterminePlacement)target;

        if (GUILayout.Button("Execute Fly Spawning"))
        {

            ((DeterminePlacement)target).SpawnObject(); // Call the function when the button is clicked
        }
    }
}
