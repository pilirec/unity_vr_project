using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class createSelf : MonoBehaviour
{

    public GameObject selected;


    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("create"))
            Instantiate(selected, new Vector3(Random.value*5, Random.value*5, Random.value*5), Quaternion.identity);
    }
}
