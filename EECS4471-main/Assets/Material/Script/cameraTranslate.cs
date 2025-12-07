using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraTranslate : MonoBehaviour
{
    // Start is called before the first frame update
   

    // Update is called once per frame
    void Update()
    {   
        Vector3 inputDirection = new Vector3(0,0,0);

            if(Input.GetKey(KeyCode.W))
                inputDirection.z = +1f;
            if(Input.GetKey(KeyCode.X))
                inputDirection.z = -1f;
            if(Input.GetKey(KeyCode.A))
                inputDirection.x = +1f;
            if(Input.GetKey(KeyCode.D))
                inputDirection.x = -1f;
            if(Input.GetKey(KeyCode.F))
                inputDirection.y = +1f;
            if(Input.GetKey(KeyCode.V))
                inputDirection.y = -1f;
  


        Vector3 finalDirection = transform.forward*inputDirection.z+transform.up*inputDirection.y+transform.right*inputDirection.x;

        float speed = 30f;
        transform.position += finalDirection*speed*Time.deltaTime;


    }
}
