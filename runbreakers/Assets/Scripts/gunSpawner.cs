using UnityEngine;
using UnityEngine.AI;

public class gunSpawner : MonoBehaviour
{
    [SerializeField] GameObject SMG;
    [SerializeField] GameObject sniper;
    [SerializeField] int spawnRate;
    [SerializeField] int spawnDist;

    float spawnTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnRate)
        {
            spawn();
        }
    }


    void spawn()
    {
        int roll = Random.Range(0, 100);
        if (0 < roll && roll <= 15)
        {
            spawnSMG();
        }
        else if (95 < roll && roll <= 100)
        {
            spawnSniper();
        }
        else
        {
            spawnTimer = 0;
        }

    }

    void spawnSMG()
    {
        spawnTimer = 0;

        Vector3 randomPos = Random.insideUnitSphere * spawnDist;
        randomPos += transform.position;

        NavMeshHit hit;
        NavMesh.SamplePosition(randomPos, out hit, spawnDist, 1);

        Instantiate(SMG, hit.position, Quaternion.Euler(0, Random.Range(0, 360), 0));
    }

    void spawnSniper()
    {
        spawnTimer = 0;

        Vector3 randomPos = Random.insideUnitSphere * spawnDist;
        randomPos += transform.position;

        NavMeshHit hit;
        NavMesh.SamplePosition(randomPos, out hit, spawnDist, 1);

        Instantiate(sniper, hit.position, Quaternion.Euler(0, Random.Range(0, 360), 0));
    }
}