using UnityEngine;

public class DeterminePlacement : MonoBehaviour
{
    //public Transform[] spawnPoints;
    public WeightedObject[] objectPool;
    public GameObject spritePrefab;
    [Header("Fly Spawning Variables")]
    [Tooltip("X Range for Fly Spawning in non hotspots")]
    public float xRange = .05f;
    [Tooltip("Y Range for Fly Spawning in non hotspots")]
    public float yRange = .95f;
    [Tooltip("How many flies to spawn")]
    [Range(0, 50)] public int FlyCount;

    public static DeterminePlacement Instance;

    public void Start()
    {

    }

    public void Awake()
    {
        Instance = this;
    }

    public void SpawnObject()
    {
        //Debug.Log("Spawning Flies");
        // Select a weighted object

        var hotspots = GameObject.FindObjectsOfType<Hotspot>(false);
        foreach(var hotspot in hotspots)
        {
            hotspot.StartRadar(Mathf.CeilToInt((float)FlyCount / (float)hotspots.Length));
        }
        //for (int i = 0; i < FlyCount; i++)
        //{
            //WeightedObject areaToSpawnFlyAt = GetRandomWeightedObject(objectPool);
            //if (areaToSpawnFlyAt.type == AreaType.Hotspot)
            //{
            //    //RandomSpawn randomSpawn = areaToSpawnFlyAt.GetComponent<RandomSpawn>();
            //    //if (randomSpawn != null && WeaveBoardManager.instance != null)
            //    //{
            //    //    for (int attempt = 0; attempt < 30; attempt++)
            //    //    {
            //    //        Vector2 randomCirclePoint = Random.insideUnitCircle * randomSpawn.spawnRadius;
            //    //        Vector3 candidatePosition = areaToSpawnFlyAt.transform.position + new Vector3(randomCirclePoint.x, randomCirclePoint.y, 0f);
            //    //        if (WeaveBoardManager.instance.IsPositionWithinWeb(candidatePosition) && !WeaveBoardManager.instance.IsPositionWithinFly(candidatePosition))
            //    //        {
            //    //            Instantiate(spritePrefab, candidatePosition, Quaternion.identity, GameManager.Instance.BoardRoot.transform);
            //    //            break;
            //    //        }
            //    //    }
            //    //}
            //    // ping pong
            //    areaToSpawnFlyAt.GetComponent<Hotspot>().StartRadar(1);
            //}
            //else
            //{
            //    float randomX = Random.Range(xRange, yRange);
            //    float randomY = Random.Range(xRange, yRange);
            //    Vector2 spawnPosition = Camera.main.ViewportToWorldPoint(new Vector3(randomX, randomY));
            //    Instantiate(spritePrefab, spawnPosition, Quaternion.identity, GameManager.Instance.BoardRoot.transform);
            //}
        //}
       
    }

    public WeightedObject GetRandomWeightedObject(WeightedObject[] weightedObjects)
    {
        float totalWeight = 0f;
        float randomValue = Random.Range(0f, 1f);
        //Debug.Log(randomValue.ToString());

        foreach (WeightedObject wo in weightedObjects)
        {
            totalWeight += wo.weight;
            if (randomValue <= totalWeight)
            {
                //Debug.Log("weighted object found: " + wo.type.ToString());
                return wo;
            }
            //Debug.Log(totalWeight.ToString());
        }
        //Debug.Log("weighted object not found");
        return null; // Should not happen if totalWeight is calculated correctly
    }
}
