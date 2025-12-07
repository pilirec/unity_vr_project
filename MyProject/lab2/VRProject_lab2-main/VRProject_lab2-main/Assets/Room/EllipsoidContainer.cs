using UnityEngine;

public class EllipsoidContainer : MonoBehaviour
{
    public Vector3 center = Vector3.zero;
    public Vector3 radii = new Vector3(2f, 1f, 3f); // X, Y, Z radii

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 direction = (center - other.transform.position).normalized;
                rb.linearVelocity = direction * rb.linearVelocity.magnitude; // Reflect inward
            }
        }
    }
}
