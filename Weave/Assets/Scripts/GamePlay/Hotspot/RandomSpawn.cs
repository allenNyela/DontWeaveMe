using UnityEngine;

public class RandomSpawn : MonoBehaviour
{


    public GameObject spritePrefab;
    public float spawnRadius; 

    void Start()
    {

        
    }

    private void OnValidate()
    {
        spawnRadius = GetComponent<CircleCollider2D>().radius;
    }

    public void SpawnSpriteRandomly()
    {
        //Debug.Log(spawnRadius.ToString());
        Vector2 randomCirclePoint = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPosition = transform.position + new Vector3(randomCirclePoint.x, randomCirclePoint.y, 0f); // Add spawner's position as offset
        //Vector3 spawnPosition = new Vector3(randomCirclePoint.x, randomCirclePoint.y, 0f);

        Instantiate(spritePrefab, spawnPosition, Quaternion.identity);
    }
}
