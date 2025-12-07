using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    public Transform mainCamera;        // main camera 
    public Transform followTarget;      // projectile position 
    public Transform launcherPosition;  // launcher position 
    public Transform targetPosition;    // Target position 
    private bool followModeProj = false;
    private bool followModeTar = false;

    void Update()
    {
        // Toggle follow mode for projectile with the C key
        if (Input.GetKeyDown(KeyCode.C))
        {
            followModeProj = !followModeProj;
            if (followModeProj)
            {
                // Attach the camera to the projectile
                mainCamera.transform.SetParent(followTarget);
                mainCamera.transform.position = followTarget.position; // set position 
                mainCamera.transform.rotation = Quaternion.identity; // Reset rotation 
                mainCamera.transform.LookAt(launcherPosition.position);   
                mainCamera.forward = -followTarget.forward; // look at camera 
            }
            else
            {
                // Detach and reset to default view beside the launcher
                mainCamera.transform.SetParent(launcherPosition);
                mainCamera.transform.position = launcherPosition.position; // launcher position
                mainCamera.transform.LookAt(followTarget); // Look at the projectile 
                mainCamera.transform.Rotate(Vector3.up * 180, Space.World);
            }
        }

        // Switch to the target position with the K key
        if (Input.GetKeyDown(KeyCode.K))
        {
            followModeTar = !followModeTar;
            if (followModeTar) {
                // Set the camera to target position facing back at the launcher
                mainCamera.transform.SetParent(targetPosition);
                mainCamera.transform.position = targetPosition.position;  // Position at the target
                mainCamera.transform.LookAt(launcherPosition.position);    // Look back at the launcher
                mainCamera.forward = -targetPosition.forward;
            } else {
                 // Detach and reset to default view beside the launcher
                mainCamera.transform.SetParent(launcherPosition);
                mainCamera.transform.position = launcherPosition.position; // launcher position
                mainCamera.transform.LookAt(targetPosition); // Look at the target 
                mainCamera.transform.Rotate(Vector3.up * 180, Space.World);
            }
        } 

    }

    // Method to set the follow target from the LauncherController
    public void SetFollowTarget(Transform newFollowTarget)
    {
        followTarget = newFollowTarget;
    }
}

