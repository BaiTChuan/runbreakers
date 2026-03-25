using UnityEngine;

public class gunPickup : MonoBehaviour
{
    [SerializeField] gunStats gun;

    private void OnTriggerEnter(Collider other)
    {
        IPickup pik = other.GetComponent<IPickup>();

        if (pik != null) {
            pik.getGun(gun);
            Destroy(gameObject);
        }
    }
}
