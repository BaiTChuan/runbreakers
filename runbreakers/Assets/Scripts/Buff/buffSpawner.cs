using UnityEngine;
using UnityEngine.AI;

public class spawner : MonoBehaviour
{
    [SerializeField] GameObject attackBuff;
    [SerializeField] GameObject speedBuff;
    [SerializeField] GameObject healthBuff;
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
        if (0 < roll && roll <= 10) {
            spawnAttack();
        }
        else if (10 < roll && roll <= 20) {
            spawnSpeed();
        }
        else if (30 < roll && roll <= 40) {
            spawnHealth();
        } 
        else
        {
            spawnTimer = 0;
        }

    }

    void spawnAttack()
    {
        spawnTimer = 0;

        Vector3 randomPos = Random.insideUnitSphere * spawnDist;
        randomPos += transform.position;

        NavMeshHit hit;
        NavMesh.SamplePosition(randomPos, out hit, spawnDist, 1);

        Instantiate(attackBuff, hit.position, Quaternion.Euler(0, Random.Range(0, 360), 0));
    }

    void spawnSpeed()
    {
        spawnTimer = 0;

        Vector3 randomPos = Random.insideUnitSphere * spawnDist;
        randomPos += transform.position;

        NavMeshHit hit;
        NavMesh.SamplePosition(randomPos, out hit, spawnDist, 1);

        Instantiate(speedBuff, hit.position, Quaternion.Euler(0, Random.Range(0, 360), 0));
    }

    void spawnHealth()
    {
        spawnTimer = 0;

        Vector3 randomPos = Random.insideUnitSphere * spawnDist;
        randomPos += transform.position;

        NavMeshHit hit;
        NavMesh.SamplePosition(randomPos, out hit, spawnDist, 1);

        Instantiate(healthBuff, hit.position, Quaternion.Euler(0, Random.Range(0, 360), 0));
    }
}