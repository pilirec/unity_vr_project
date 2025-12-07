using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lightModification : MonoBehaviour
{
    // Start is called before the first frame update

    public Light myLight;

    void Start()
    {
        myLight = GetComponent<Light>();

    }

    // Update is called once per frame
    void Update()
    {
        float intensitySpeed = 0.1f;

        float intensityMyLight = 0f;

        if(Input.GetKey(KeyCode.U))
            intensityMyLight = +1f;
        if(Input.GetKey(KeyCode.O))
            intensityMyLight = -1f;
        

        myLight.intensity += intensityMyLight*intensitySpeed;
    
    }
}
