using UnityEngine;

public class SpotLightController : MonoBehaviour
{
    public Light spotLight;
    public float intensityStep = 0.2f;
    public float rotationSpeed = 50f;

    void Update()
    {
        if (spotLight == null) return;

        // Adjust brightness (Intensity)
        if (Input.GetKey(KeyCode.Alpha3))
            spotLight.intensity += intensityStep * Time.deltaTime;

        if (Input.GetKey(KeyCode.Alpha4))
            spotLight.intensity = Mathf.Max(0, spotLight.intensity - intensityStep * Time.deltaTime);

        // Adjust orientation (Rotation)
        float rotX = 0f, rotY = 0f, rotZ = 0f;

        if (Input.GetKey(KeyCode.I)) rotX = -rotationSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.K)) rotX = rotationSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.J)) rotY = -rotationSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.L)) rotY = rotationSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.U)) rotZ = -rotationSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.O)) rotZ = rotationSpeed * Time.deltaTime;

        transform.Rotate(rotX, rotY, rotZ, Space.Self);
    }
}
