using UnityEngine;

public class CollectibleItems : MonoBehaviour
{

    [SerializeField] string targetID = "CollectibleItem";
    [SerializeField] float pickupRadius = 1.5f;
    [SerializeField] AudioClip pickupSound;
    [SerializeField] AudioSource audioSource;
    Transform player;

    void Start()
    {
        if (Gamemanager.instance != null && Gamemanager.instance.player != null)
            player = Gamemanager.instance.player.transform;
        
    }

    void Update()
    {

        if (player == null)
            return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= pickupRadius)
        {
            if (questManager.instance != null)
                questManager.instance.ReportItemCollected(targetID);

            if (pickupSound != null && audioSource != null)
                audioSource.PlayOneShot(pickupSound);

            Destroy(gameObject);
        }

    }



    }

   

        
