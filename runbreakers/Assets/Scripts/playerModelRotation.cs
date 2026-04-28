using UnityEngine;

public class LookAtCursor : MonoBehaviour
{
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Plane groundPlane = new Plane(Vector3.up, transform.position);

        if (groundPlane.Raycast(ray, out float rayDistance))
        {
            Vector3 pointToLook = ray.GetPoint(rayDistance);
            Vector3 lookDirection = pointToLook - transform.position;
            lookDirection.y = 0f; 

            if (lookDirection.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.LookRotation(lookDirection);
                transform.localRotation *= Quaternion.Euler(0, 90, 0);
            }
        }
    }
}