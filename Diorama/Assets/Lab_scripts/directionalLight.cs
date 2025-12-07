using UnityEngine;

public class DirectionalLightController : MonoBehaviour
{
    public Light directionalLight;
    public float intensityStep = 0.2f;
    public float rotationSpeed = 50f;

    void Update()
    {
        if (directionalLight == null) return;

        // Adjust brightness (Intensity)
        if (Input.GetKey(KeyCode.Alpha1))
            directionalLight.intensity += intensityStep * Time.deltaTime;

        if (Input.GetKey(KeyCode.Alpha2))
            directionalLight.intensity = Mathf.Max(0, directionalLight.intensity - intensityStep * Time.deltaTime);

        // Adjust orientation (Rotation)
        float rotX = 0f, rotY = 0f, rotZ = 0f;

        if (Input.GetKey(KeyCode.G)) rotX = -rotationSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.B)) rotX = rotationSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.V)) rotY = -rotationSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.N)) rotY = rotationSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.F)) rotZ = -rotationSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.H)) rotZ = rotationSpeed * Time.deltaTime;

        transform.Rotate(rotX, rotY, rotZ, Space.World);
    }
}
