using UnityEngine;

public class enemyAI : MonoBehaviour
{
    [Header("---- AI Settings ----")]
    [SerializeField] float moveSpeed = 2f;

    Transform player;

    void Start()
    {
        if (gamemanager.instance != null)
        {
            player = gamemanager.instance.player.transform;
        }
    }

    void Update()
    {
        if (player == null)
            return;

            MoveTowardsPlayer();
    }

void MoveTowardsPlayer()
    {
    Vector3 direction = player.position - transform.position;
    direction.y = 0;

    transform.position += direction.normalized * moveSpeed * Time.deltaTime;

     if (direction != Vector3.zero)
       {
        transform.rotation = Quaternion.LookRotation(direction);
       }
    }
}