using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroySelf : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject selected;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("kill"))
            Destroy(selected);
    }
}
