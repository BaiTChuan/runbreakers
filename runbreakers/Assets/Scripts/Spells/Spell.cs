using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]
public class Spell : MonoBehaviour
{
    public ProjectileSpellScriptableObject spellToCast;

    private SphereCollider spellCollider;
    private Rigidbody spellRigidbody;

    private void Awake()
    {
        spellCollider = GetComponent<SphereCollider>();
        spellCollider.isTrigger = true;
        spellCollider.radius = spellToCast.spellRadius;

        spellRigidbody = GetComponent<Rigidbody>();
        spellRigidbody.useGravity = false;
        spellRigidbody.isKinematic = true;

        Destroy(this.gameObject, spellToCast.lifeTime);
    }

    private void Update()
    {
        if (spellToCast.speed > 0) transform.Translate(Vector3.forward * spellToCast.speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {

        Destroy(this.gameObject);
    }
}
