using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraRotation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float rotateDirctionX = 0f;
        float rotateDirctionY = 0f;
        float rotateDirctionZ = 0f;

        float speed = 30f;

         if(Input.GetKey(KeyCode.D))
           rotateDirctionY = +1f;
         if(Input.GetKey(KeyCode.A))
            rotateDirctionY = -1f;

        if(Input.GetKey(KeyCode.F))
           rotateDirctionX = +1f;
        if(Input.GetKey(KeyCode.V))
            rotateDirctionX = -1f;

        if(Input.GetKey(KeyCode.W))
           rotateDirctionZ = +1f;
        if(Input.GetKey(KeyCode.X))
            rotateDirctionZ = -1f;

        transform.eulerAngles += new Vector3(rotateDirctionX*speed*Time.deltaTime, rotateDirctionY*speed*Time.deltaTime, rotateDirctionZ*speed*Time.deltaTime);

    }
}
