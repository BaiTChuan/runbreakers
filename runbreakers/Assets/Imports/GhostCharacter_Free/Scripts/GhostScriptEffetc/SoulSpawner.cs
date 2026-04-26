using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class SoulSpawner : MonoBehaviour
{
    [SerializeField] GameObject soulPrefab;
    [SerializeField] int maxSoulsAtOnce = 5;
    [SerializeField] float spawnInterval = 8f;
    [SerializeField] float spawnRadius = 50f;
    [SerializeField] AudioClip[] eerieAmbience;
    [SerializeField] AudioSource ambientSource;

    int activeSouls = 0;

    void Start()
    {
        StartCoroutine(SpawnLoop());
        StartCoroutine(PlayEerieAmbience());
        
    }

    IEnumerator PlayEerieAmbience()
    {
        while(true)
        {
            if (eerieAmbience.Length > 0 && ambientSource != null)
            {
                AudioClip clip = eerieAmbience[Random.Range(0, eerieAmbience.Length)];
                ambientSource.PlayOneShot(clip);
                yield return new WaitForSeconds(clip.length + Random.Range(spawnInterval, spawnRadius));

            }
            else
                yield return new WaitForSeconds(5f);
        }
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return null;

            if (activeSouls < maxSoulsAtOnce)
                TrySpawnSoul();
        }
    }

    void TrySpawnSoul()
    {
        Vector3 center = Vector3.zero;

        if (Gamemanager.instance != null && Gamemanager.instance.player != null)
            center = Gamemanager.instance.player.transform.position;

        for (int i = 0; i < 10; i++)
        {
            Vector2 random = Random.insideUnitSphere * spawnRadius;
            Vector3 guess = center + new Vector3(random.x, 0f, random.y);

            NavMeshHit hit;
            if (NavMesh.SamplePosition(guess, out hit, 5f, NavMesh.AllAreas))
            {
                GameObject soul = Instantiate(soulPrefab, hit.position, Quaternion.identity);
                activeSouls++;
                StartCoroutine(TrackSoul(soul));
                return;
            }
        }
    }

    IEnumerator TrackSoul(GameObject soul)
    {
        while (soul != null)
            yield return null;

        activeSouls--;
    }



}
