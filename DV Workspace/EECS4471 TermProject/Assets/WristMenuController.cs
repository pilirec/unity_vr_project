using UnityEngine;

public class WristMenuToggle : MonoBehaviour
{
    // Assign your menu canvas in the Inspector
    public GameObject wristCanvas;

    void Update()
    {
        // B button (Joystick Button 1 on Oculus)
        if (Input.GetKeyDown(KeyCode.JoystickButton1))
        {
            wristCanvas.SetActive(!wristCanvas.activeSelf);
        }

        if (Input.anyKeyDown)
{
    for (int i = 0; i < 20; i++)
    {
        if (Input.GetKeyDown((KeyCode) (350 + i)))
        {
            Debug.Log("Joystick button " + i + " pressed");
        }
    }
}

    }
}