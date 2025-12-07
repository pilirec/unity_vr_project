
using UnityEngine;

public class cameraFollow : MonoBehaviour
{
    public Transform target;

    public float smoothSpeed = 0.15f;

    public Vector3 offset;

    void FixedUpdate()
    {
        Vector3 behindPosition = target.position + offset;
        Vector3 smoothPosition = Vector3.Lerp(transform.position, behindPosition, smoothSpeed);
        transform.position = smoothPosition;
    }
    // Start is called before the first frame update
    
}
