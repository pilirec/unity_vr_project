using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTranslateSwitch : MonoBehaviour
{
   public GameObject camera;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.T))
            {
                camera.GetComponent<cameraTranslate>().enabled = true;
                camera.GetComponent<cameraRotation>().enabled = false;
            }

        if(Input.GetKey(KeyCode.R))
            {
                camera.GetComponent<cameraTranslate>().enabled = false;
                camera.GetComponent<cameraRotation>().enabled = true;
            }
         
    }
}
