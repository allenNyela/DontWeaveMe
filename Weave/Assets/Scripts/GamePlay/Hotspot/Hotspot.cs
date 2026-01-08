using System.Collections.Generic;
using UnityEngine;

public class Hotspot : MonoBehaviour
{
    public float bounceTime = 5f;

    public float flySpeed = 1f;

    public GameObject radarPrefab;

    //get current range collider and bounce this shit within it

    public CircleCollider2D bounceBoundary;

    //private Vector2 velocity;
    //private bool stopped = true;

    //private float timer;

    private int radarCount;

    private List<RadarData> radarDatas = new List<RadarData>();
    public float randomBounceAngle = 30f;
    private class RadarData
    {
        public GameObject go;
        public float timer;
        public Vector2 velocity;
        public bool stopped;
    }

    public void StartRadar(int count)
    {
        radarDatas = new List<RadarData>();
        radarCount = 0;
        while (radarCount < count)
        {
            radarCount++;
            var data = new RadarData();
            Vector2 dir = Random.insideUnitCircle.normalized;
            data.velocity = dir * flySpeed;
            data.stopped = false;
            data.timer = 0;
            var radarObj = Instantiate(radarPrefab, this.transform.parent);
            radarObj.transform.position = this.gameObject.transform.position;
            radarObj.transform.parent = this.gameObject.transform;
            data.go = radarObj;

            radarDatas.Add(data);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void OnRadarStop(RadarData data)
    {
        //generate fly
        if (WeaveBoardManager.instance.IsPositionWithinWeb(data.go.transform.position))
        {
            WeaveBoardManager.instance.SpawnFly(data.go.transform.position);
        }
        GameObject.Destroy(data.go);
    }

    // Update is called once per frame
    void Update()
    {
        foreach(var data in radarDatas)
        {
            if (data.stopped)
                continue;

            data.timer += Time.deltaTime;

            if (data.timer >= bounceTime)
            {
                data.velocity = Vector2.zero;
                data.stopped = true;
                //callback
                OnRadarStop(data);
                return;
            }

            var radarObj = data.go;
            Vector2 nextPos = (Vector2)radarObj.transform.position + data.velocity * Time.deltaTime;

            // CircleCollider 世界中心 & 半径
            Vector2 center = bounceBoundary.transform.position;
            float radius = bounceBoundary.radius * bounceBoundary.transform.lossyScale.x;

            Vector2 toNext = nextPos - center;
            Vector2 pos = radarObj.transform.position;

            // 撞到边界 → 反弹
            if (toNext.magnitude > radius)
            {
                Vector2 normal = toNext.normalized;
                data.velocity = ReflectWithRandomness(
                    data.velocity,
                    pos,
                    center,
                    randomBounceAngle
                );
                nextPos = (Vector2)radarObj.transform.position + data.velocity * Time.deltaTime;
            }

            radarObj.transform.position = nextPos;
        }




    }

    Vector2 ReflectWithRandomness(
        Vector2 velocity,
        Vector2 position,
        Vector2 center,
        float randomAngleDeg
    )
    {
        Vector2 normal = (position - center).normalized;
        Vector2 reflected = Vector2.Reflect(velocity, normal).normalized;

        float angle = Random.Range(-randomAngleDeg, randomAngleDeg);
        Vector2 randomized = Quaternion.Euler(0, 0, angle) * reflected;

        return randomized.normalized * velocity.magnitude;
    }
}
