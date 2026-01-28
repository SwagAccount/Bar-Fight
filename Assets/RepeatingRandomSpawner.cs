using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepeatingRandomSpawner : MonoBehaviour
{
    public List<GameObject> Prefabs;
    public Vector2 SpawnTime = new Vector3(0.5f, 2);
    public TimeSince lastSpawn;
    private float nextSpawn;
    // Start is called before the first frame update
    void Start()
    {
        nextSpawn = Random.Range(SpawnTime.x, SpawnTime.y);
    }

    // Update is called once per frame
    void Update()
    {
        if (lastSpawn < nextSpawn)
            return;

        var go = Instantiate(Prefabs[Random.Range(0, Prefabs.Count)]);
        go.transform.position = transform.position;
        go.transform.rotation = transform.rotation;

        nextSpawn = Random.Range(SpawnTime.x, SpawnTime.y);
        lastSpawn = 0;
    }
}
