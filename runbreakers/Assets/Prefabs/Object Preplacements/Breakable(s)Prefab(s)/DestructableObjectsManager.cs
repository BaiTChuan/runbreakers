using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DestructableObjectsManager : MonoBehaviour
{

    public static DestructableObjectsManager instance;

    [SerializeField] GameObject destructableObject;
    [SerializeField] int maxActive = 15;
    [SerializeField] float respawnDelay = 5f;


    List<Vector3> spawnPoints = new List<Vector3>();
    int activeCount = 1;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        StartCoroutine(DelayedStart());
    }

    void GenerateSpawnPoints()
    {
        Vector3 center = Vector3.zero;

        if (Gamemanager.instance != null && Gamemanager.instance.player != null)
            center = Gamemanager.instance.player.transform.position;
        for (int i = 0; i < 3; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * 40f;
            Vector3 spawnGuess = center + new Vector3(randomCircle.x, 0f, randomCircle.y);

            NavMeshHit hit;
            if (NavMesh.SamplePosition(spawnGuess, out hit, 5f, NavMesh.AllAreas))
            {
                spawnPoints.Add(hit.position);
            }

        }
      
        Debug.Log("Generated " + spawnPoints.Count + "spawn points!");
    }

    void SpawnInitial()
    {
        if (spawnPoints.Count == 0)
        {
            Debug.LogWarning("No spawn points generated" + spawnPoints.Count);
            return;
        }

        List<int> indices = new List<int>();
        for (int i = 0; i < spawnPoints.Count; i++)
            indices.Add(i);

        for (int i = 0; i < indices.Count; i++)
        {
            int rand = Random.Range(i, indices.Count);
            (indices[i], indices[rand]) = (indices[rand], indices[i]);
        }

        int toSpawn = Mathf.Min(maxActive, spawnPoints.Count);
        for (int i = 0; i < toSpawn; i++) 
            SpawnAt(spawnPoints[indices[i]]);
            
        
    }
    void SpawnAt(Vector3 position)
    {
        Vector3 spawnPos = new Vector3(position.x, 0.5f, position.z);
        Instantiate(destructableObject,spawnPos, Quaternion.identity);
        activeCount++;
    }

    public void OnDestructableDestroyed()
    {
        activeCount--;
        StartCoroutine(RespawnAfterDelay());
    }

    IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(respawnDelay);

      
    }

    IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(0.5f);
        GenerateSpawnPoints();
        SpawnInitial();
    }
}
