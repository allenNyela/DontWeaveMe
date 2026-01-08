using UnityEngine;


public enum AreaType
{
    Hotspot,
    NonHotspot
}

[System.Serializable]
public class WeightedObject : MonoBehaviour
{
    [SerializeField]
    [Range (0f, 1f)]public float weight; // Relative weight for spawning flys at this object
    [SerializeField]
    public AreaType type;
}
