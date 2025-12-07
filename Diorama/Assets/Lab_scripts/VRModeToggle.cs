using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;

public class VRModeToggle : MonoBehaviour
{
    private XRDisplaySubsystem xrDisplay; 

    void Start()
    {
        // Get the XR Display Subsystem Objects
        List<XRDisplaySubsystem> displaySubsystems = new List<XRDisplaySubsystem>();
        SubsystemManager.GetSubsystems(displaySubsystems);
        
        if (displaySubsystems.Count > 0)
        {
            xrDisplay = displaySubsystems[0];
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V)) // Press 'V' to toggle VR Mode
        {
            ToggleVR();
        }
    }

    void ToggleVR()
    {
        if (xrDisplay != null)
        {
            if (xrDisplay.running)
            {
                xrDisplay.Stop(); // Exit VR Mode
                Debug.Log("VR Mode Stopped");
            }
            else
            {
                xrDisplay.Start(); // Enter VR Mode
                Debug.Log("VR Mode Started");
            }
        }
        else
        {
            Debug.LogError("XR Display Subsystem not found!");
        }
    }
}
