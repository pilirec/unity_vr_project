using UnityEngine;

public class LauncherController : MonoBehaviour
{
    public Rigidbody projectilePrefab;     // sphere projectile asset
    public Transform launchPoint;          // The point from where the projectile is spawned
    public float launchForce = 10f;        // force of launcher 
    public float torqueForce = 5f;         // torque applied 
    private Rigidbody currentProjectile;   // Keep track of the current projectile
    public CameraSwitch cameraSwitchScript;  // Reference to CameraSwitch script
 
    void Start()
    {
        launchPoint.forward = transform.forward; // initilize launcher point at end of launcher tip 
    }
    
    // Update is called once per frame
    void Update()
    {
        // Rotate the launcher using left/right arrow keys
        if (Input.GetKey(KeyCode.LeftArrow))
            transform.Rotate(Vector3.up * -0.1f, Space.World);
        if (Input.GetKey(KeyCode.RightArrow))
            transform.Rotate(Vector3.up * 0.1f, Space.World);
        
        // Adjust launch force with up/down arrow keys
        if (Input.GetKey(KeyCode.UpArrow))
            launchForce += 0.1f;
        if (Input.GetKey(KeyCode.DownArrow))
            launchForce = Mathf.Max(0, launchForce - 0.1f);
        
        // Press Space to launch projectile
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (currentProjectile != null)
            {
                Destroy(currentProjectile.gameObject); // delete last projectile so only 1 projectile is in scene at a time
            }

            // spawn and apply force on projectile 
            currentProjectile = Instantiate(projectilePrefab, launchPoint.position, launchPoint.rotation);
            currentProjectile.AddForce(- launchPoint.forward * launchForce, ForceMode.Impulse);
            currentProjectile.AddForce(launchPoint.up * launchForce, ForceMode.Impulse);
            
            // If the T key is held down, additional torque is applied to projectile 
            if (Input.GetKey(KeyCode.T))
                currentProjectile.AddTorque(Random.insideUnitSphere * torqueForce, ForceMode.Impulse);
            
            // Pass new projectile to the CameraSwitch script for swtiching view points when needed 
            if (cameraSwitchScript != null)
            {
                cameraSwitchScript.SetFollowTarget(currentProjectile.transform); 
            }
        }
    }
}
