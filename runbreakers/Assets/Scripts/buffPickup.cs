using UnityEngine;

public class buffPickup : MonoBehaviour
{
    [SerializeField] buffStats buff;

    private void OnTriggerEnter(Collider other)
    {
        IBuff pick = other.GetComponent<IBuff>();

        if (pick != null)
        {
            pick.getBuff(buff);
            Destroy(gameObject);
        }
    }
}
