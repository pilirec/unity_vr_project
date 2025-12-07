using UnityEngine;

public class XRInitial : MonoBehaviour
{
    public GameObject launcher;  

    void Start()
    {
        if (launcher == null)
        {
            Debug.LogError("Launcher is not assigned in XRInitial script!");
            return;
        }

        transform.position = launcher.transform.position;
        // transform.rotation = launcher.transform.rotation;  // Align rotation too
        //transform.rotation = Quaternion.Euler(0, 180, 0);  // Rotate Camera to face forward

        // move vr camera with launcher
        transform.SetParent(launcher.transform);
    }
}
