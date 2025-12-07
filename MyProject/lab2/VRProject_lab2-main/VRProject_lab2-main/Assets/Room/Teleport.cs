using UnityEngine;

public class Teleport : MonoBehaviour
{
    public Transform camera;
    public Transform target;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Transfer()
    {
        camera.position = target.position;
    }
}
