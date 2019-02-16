using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Vector3 offset;
    void LateUpdate()
    {
        transform.position = transform.parent.position + new Vector3(offset.x, offset.y, offset.z);
        transform.rotation = Quaternion.Euler(90, 0, 0);
    }
}
