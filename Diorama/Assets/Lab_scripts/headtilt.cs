using UnityEngine;
using UnityEngine.XR;

public class headtilt : MonoBehaviour
{
    public Transform headset; // Assign the XR camera (usually the "Main Camera")
    public Transform launcher; // Assign launcher object

    private bool isGazeActive = false; // checks weather to use gaze for aiming

    private Vector3 initialLocalPosition;
    private Quaternion initialLocalRotation;
    private bool initialized = false;
    public float rotationFollowStrength = 0.1f; // rotation speed of headset

    void Start()
    {
        // Set Initial launcher position 
        initialLocalPosition = launcher.localPosition;
        initialLocalRotation = launcher.localRotation;
        initialized = true;
    }

    void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.G)) // Toggle gaze mode via 'G'
        {
            isGazeActive = !isGazeActive;
        }

        if (isGazeActive && headset != null && launcher != null)
        {
            // Match rotation with the camera's forward direction
            // Interpolate between current rotation and camera rotation based on follow strength
            Quaternion targetRotation = Quaternion.Lerp(launcher.rotation, 
                                                    headset.rotation, 
                                                    rotationFollowStrength);
            launcher.transform.rotation = targetRotation;
        }
    }
    
}
