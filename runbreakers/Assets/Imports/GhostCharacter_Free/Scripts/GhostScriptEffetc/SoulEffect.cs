using System.Collections;
using UnityEngine;

public class SoulEffect : MonoBehaviour
{
    [SerializeField] float floatSpeed = 0.5f;
    [SerializeField] float lifetime = 3f;
    [SerializeField] float fadeDelay = 1.5f;
    [SerializeField] float swaySpeed = 2f;
    [SerializeField] float swayAmount = 0.5f;
    Renderer[] renderers;

    void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();
        StartCoroutine(FloatAndFade());
    }

    IEnumerator FloatAndFade()
    {
        float timer = 0f;
        while (timer < lifetime)
        {
            timer += Time.deltaTime;
            transform.position += Vector3.up * floatSpeed * Time.deltaTime;
            float sway = Mathf.Sin(timer * swaySpeed) * swayAmount * Time.deltaTime;
            transform.position += transform.right * sway;
            if (timer >= fadeDelay)
            {
                float alpha = Mathf.Lerp(0f, 1f, (timer - fadeDelay) / (lifetime - fadeDelay));
                foreach (Renderer r in renderers)
                {
                    foreach (Material m in r.materials)
                    {
                        Color c = m.color;
                        c.a = alpha;
                        m.color = c;
                    }
                }

            }

            yield return null;
        }

        Destroy(gameObject);
    }
}
