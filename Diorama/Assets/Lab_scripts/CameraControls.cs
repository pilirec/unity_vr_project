using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 100f;

    private bool isTranslationMode = true; // Start in translation mode

    void Update()
    {
        // Switch modes with "T" (translation) and "R" (rotation)
        if (Input.GetKeyDown(KeyCode.T))
            isTranslationMode = true;
        if (Input.GetKeyDown(KeyCode.R))
            isTranslationMode = false;

        if (isTranslationMode)
        {
            HandleTranslation();
        }
        else
        {
            HandleRotation();
        }
    }

    void HandleTranslation()
    {
        float moveX = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime; // A/D
        float moveZ = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime; // W/S

        // Additional Up/Down movement using Q/E keys
        float moveY = 0f;
        if (Input.GetKey(KeyCode.Q)) moveY = -moveSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.E)) moveY = moveSpeed * Time.deltaTime;

        transform.Translate(new Vector3(moveX, moveY, moveZ));
    }

    void HandleRotation()
    {
        float rotX = 0f, rotY = 0f, rotZ = 0f;

        if (Input.GetKey(KeyCode.W)) rotX = rotationSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.S)) rotX = -rotationSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.A)) rotY = -rotationSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.D)) rotY = rotationSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.Q)) rotZ = -rotationSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.E)) rotZ = rotationSpeed * Time.deltaTime;

        // Convert to Quaternion and apply rotation
        Quaternion deltaRotation = Quaternion.Euler(rotX, rotY, rotZ);
        transform.rotation = transform.rotation * deltaRotation;
    }
}
