using UnityEngine;
public class fixBounds : MonoBehaviour
{
    void Start()
    {
        SkinnedMeshRenderer smr = GetComponent<SkinnedMeshRenderer>();
        if (smr != null)
        {
            smr.updateWhenOffscreen = true;
        }
    }
}
