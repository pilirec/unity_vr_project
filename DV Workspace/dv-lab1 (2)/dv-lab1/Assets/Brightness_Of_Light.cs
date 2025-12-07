using UnityEngine;

public class Brightness_Of_Light : MonoBehaviour
{
    public Light sceneLight; //The light component we need to get
    public float intensityStep = 500000000f;
    public float defaultIntensity = 0;

    void ResetLightIntensity()
    {
        if (sceneLight != null)
        {
            sceneLight.intensity = defaultIntensity;
            Debug.Log("Reset Light");
        }
    }

    void Start()
    {
        if(sceneLight == null)
        {
            sceneLight = GetComponent<Light>();
        }
        
        // Get the default intensity. Yeah... No catch block.
        defaultIntensity = sceneLight.intensity;

    }

    // Update is called once per frame
    void Update()
    {
        if(sceneLight != null)
        {
            // Increase brightness (no upper limit)
            if (Input.GetKey(KeyCode.UpArrow))
            {
                sceneLight.intensity += intensityStep * Time.deltaTime;
            }

            // Decrease brightness (no lower limit)
            if (Input.GetKey(KeyCode.DownArrow))
            {
                sceneLight.intensity -= intensityStep * Time.deltaTime;
            }

            // Reset brightness to default level when pressing R
            if (Input.GetKeyDown(KeyCode.L))
            {
                ResetLightIntensity();
            }
        }
    }
}
