using UnityEngine;

public class FloatingGold : MonoBehaviour
{
    [SerializeField] float floatSpeed = 2f;
    [SerializeField] float lifetime = 1f;

    float timer;
    MeshRenderer mr;

    void Start()
    {
        mr = GetComponentInChildren<MeshRenderer>();
    }

    void Update()
    {
        timer += Time.deltaTime;
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        Color c = mr.material.color;
        c.a = Mathf.Lerp(1f, 0f, timer / lifetime);
        mr.material.color = c;

        if (timer >= lifetime)
            Destroy(gameObject);
    }
}

