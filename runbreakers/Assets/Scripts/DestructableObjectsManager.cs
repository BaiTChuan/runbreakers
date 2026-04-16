using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableObjectsManager : MonoBehaviour
{
    public static DestructableObjectsManager instance;

    [SerializeField] GameObject destructablePrefab;
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] int maxActive = 10;
    [SerializeField] float respawnDelay = 5f;

    int activeCount = 0;

    private void Awake()
    {
        instance = this;


    }

    private void Start()
    {
        SpawnInitial();
    }

    void SpawnInitial()
    {
        List<int> indices = new List<int>();
        for (int i = 0; i < spawnPoints.Length; i++)
            indices.Add(i);

        for (int i = 0; i < indices.Count; i++)
        {
            int rand = Random.Range(i, indices.Count);
            (indices[i], indices[rand]) = (indices[rand], indices[i]);
        }

        int toSpawn = Mathf.Min(maxActive, spawnPoints.Length);
        for (int i = 0; i < toSpawn; i++)
            SpawnAt(spawnPoints[indices[i]]);

    }

    void SpawnAt(Transform point)
    {
        Instantiate(destructablePrefab, point.position, point.rotation);
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
        Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];
        SpawnAt(point);
    }
}
