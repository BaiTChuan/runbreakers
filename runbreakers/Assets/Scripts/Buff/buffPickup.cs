using UnityEngine;

public class buffPickup : MonoBehaviour
{
    [SerializeField] buffStats buff;

    private void OnTriggerEnter(Collider other)
    {
        IPickup pick = other.GetComponent<IPickup>();

        if (pick != null)
        {
            pick.getBuff(buff);
            Destroy(gameObject);
        }
    }
}
