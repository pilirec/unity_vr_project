using UnityEngine;

public class WebcamFeed : MonoBehaviour
{
    // Assign this via the Inspector: the Renderer of your 360 sphere
    public Renderer sphereRenderer;
    private WebCamTexture webcamTexture;

    void Start()
    {
        // Check if any webcam devices are available.
        if (WebCamTexture.devices.Length > 0)
        {
            // Optionally, you can choose a specific device by name.
            string deviceName = WebCamTexture.devices[0].name;
            Debug.Log("Using webcam: " + deviceName);

            // Create a WebCamTexture using the device
            webcamTexture = new WebCamTexture(deviceName, 1920, 960, 30);
            
            // Set the texture from the webcam as the main texture for the material.
            sphereRenderer.material.SetTexture("_MainTex", webcamTexture);
            
            // Start the webcam feed.
            webcamTexture.Play();
        }
        else
        {
            Debug.LogError("No webcam device found!");
        }
    }
}
