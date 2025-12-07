using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ResetTriger : MonoBehaviour
{

    private Dictionary<Rigidbody, Vector3> initialPositions = new Dictionary<Rigidbody, Vector3>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Store the initial positions of all child spheres
        //foreach (Transform child in transform)
        //{
        //    Rigidbody rb = child.GetComponent<Rigidbody>();
        //    initialPositions[rb] = child.position;
        //}
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Ball");
        foreach (GameObject bullet in bullets)
        {
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                initialPositions[rb] = bullet.transform.position;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // This function will be called when the object is clicked
    public void OnObjectClicked()
    {
        Debug.Log("Object was clicked!");
        // Add your custom functionality here
        foreach (var entry in initialPositions)
        {
            Rigidbody rb = entry.Key;
            rb.position = entry.Value;
            rb.rotation = Quaternion.identity;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

        }
        Debug.Log("All balls reset to their initial positions.");

    }
}
