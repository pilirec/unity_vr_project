using UnityEngine;

public class camera_controls : MonoBehaviour
{

    public float moveSpeed = 5f;
    public float rotationSpeed = 100f;

    private bool isTraslationMode = true;

    void HandleTranslation()
    {
        //Init float
        float moveX = 0f;
        float moveZ = 0f;

        //Get possible movement inputs
        if (Input.GetKey(KeyCode.A)) moveX = -1f;  // Move left
        if (Input.GetKey(KeyCode.D)) moveX = 1f;   // Move right
        if (Input.GetKey(KeyCode.W)) moveZ = 1f;   // Move forward
        if (Input.GetKey(KeyCode.X)) moveZ = -1f;  // Move backward


        //Compute Translation
        Vector3 movement = new Vector3(moveX, 0, moveZ) * moveSpeed * Time.deltaTime;
        transform.Translate(movement, Space.World);
    }

    void HandleRotation()
    {
        float rotateY = 0f;
        float rotateX = 0f;

        if (Input.GetKey(KeyCode.A)) rotateY = -1f;  // Rotate left
        if (Input.GetKey(KeyCode.D)) rotateY = 1f;   // Rotate right
        if (Input.GetKey(KeyCode.W)) rotateX = -1f;  // Rotate up
        if (Input.GetKey(KeyCode.X)) rotateX = 1f;   // Rotate down

        //Compute Rotation
        Vector3 rotation = new Vector3(rotateX, rotateY, 0) * rotationSpeed * Time.deltaTime;
        transform.Rotate(rotation);

    }



    void Update()
    {
        //Select between the toggle modes:
        if (Input.GetKeyDown(KeyCode.T))
            isTraslationMode = true;
        if (Input.GetKeyDown(KeyCode.R))
            isTraslationMode = false;

        // Handle movement or rotation
        if (isTraslationMode)
            HandleTranslation();
        else
            HandleRotation();
    }
}
