using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class VRStasisEffect : MonoBehaviour
{
    public float stasisDuration = 5f;
    public float forceMultiplier = 10f; 
    public Color stasisColor = Color.yellow; // same as color in the Zelda Game 
    public float maxAccumForceMagnitude;
    public Transform camera; 
    public float smoothSpeed = 0.125f;
    public float gazeRayForce;
    public bool allowGaze = false;

    private Color oriColor;

    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor rayInteractor;
    private GameObject targetObject; 
    private Rigidbody rig;
    private Renderer rend; 
    private bool isStasised = false; 
    private bool isBinding = false;      
    private float stasisTimer = 0f; 
    private Vector3 accumulatedForce = Vector3.zero; // force stored in the frozed object
    private Vector3 offset;
    private float gazeCoolTime = 0.4f;
    private bool staring = false;

    public LineRenderer lineRenderer; 

    void Start()
    {
        rig = GetComponent<Rigidbody>();
        rend = GetComponent<Renderer>();
        oriColor = GetComponent<Renderer>().material.GetColor("_BaseColor");
        Debug.Log(oriColor);

        if (lineRenderer == null)
        {
            // create LineRenderer
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.startWidth = 0.02f;
            lineRenderer.endWidth = 0.05f;
            lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
            lineRenderer.material.color = Color.yellow;
        }

        Debug.Log("Camera position: " + camera.position);

    }

    void UpdateForceLine()
    {
        if (accumulatedForce.magnitude > 0.01f)
        {
            lineRenderer.enabled = true;
            Vector3 startPoint = transform.position;
            SupressForece();
            Vector3 endPoint = transform.position + accumulatedForce.normalized * accumulatedForce.magnitude * 0.3f;

            lineRenderer.SetPosition(0, startPoint);
            lineRenderer.SetPosition(1, endPoint);

            // change force arrow color accordign to the magnitude of force
            Color forceColor = Color.Lerp(Color.green, Color.red, accumulatedForce.magnitude / maxAccumForceMagnitude);
            lineRenderer.material.color = forceColor;
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }

    void SupressForece()
    {
        if (accumulatedForce.magnitude > maxAccumForceMagnitude)
        {
            accumulatedForce = accumulatedForce.normalized * maxAccumForceMagnitude;
        }
    }

    void Update()
    {
        // applied stasis to an object
        if (isStasised)
        {
            stasisTimer -= Time.deltaTime;
            if (stasisTimer <= 0f)
            {
                ReleaseStasis();
            }

        }
        UpdateForceLine();

        // in Gaze mode
        if (allowGaze)
        {
            if (gazeCoolTime > 0f)
            {
                gazeCoolTime -= Time.deltaTime;
            }
            ApplyForce();
        }
    }

    void LateUpdate()
    {
        if (!isStasised && isBinding)
        {
            CameraMoveWith();
        }
 
    }


    // stasis ability
    public void ApplyStasis()
    {
        // if object is frozen, you can grab the object, and move with it when it unfrozed
        if (isStasised)
        {
            isBinding = true;
            offset = camera.position - transform.position;
            return;
        }

        if (rig != null)
        {
            GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            GetComponent<Rigidbody>().isKinematic = true; 
            isStasised = true;
            stasisTimer = stasisDuration;
            accumulatedForce = Vector3.zero;

            if (rend != null)
            {
                rend.material.SetColor("_BaseColor", stasisColor);
            }

            Debug.Log("Stasis Applied!");
        }
    }
    
    void ReleaseStasis()
    {
        if (rig != null)
        {
            GetComponent<Rigidbody>().isKinematic = false;
            SupressForece();
            GetComponent<Rigidbody>().AddForce(accumulatedForce, ForceMode.Impulse); 
            isStasised = false;
            accumulatedForce = Vector3.zero;
            if (rend != null )
            {
                rend.material.SetColor("_BaseColor", oriColor);
            }

            Debug.Log("Stasis Released!");
        }
    }

    public void GazeTrigger()
    {
        Debug.Log("Triggered Gaze");
        if (allowGaze)
        {
            allowGaze = false;
        }
        else
        {
            allowGaze = true;
        }
    }

    // release the object
    public void RelesseBind()
    {
        isBinding = false;
    }

    // Let camera move with grabed object
    public void CameraMoveWith()
    {
        camera.position = transform.position + offset;

    }

    public void gazeEnter()
    {
        staring = true;
    }

    public void gazeExit()
    {
        staring = false;
    }

    public void ApplyForce()
    {
        if (!staring)
            return;

        if (gazeCoolTime <= 0f && isStasised)
        {
            Vector3 direction = transform.position - camera.position;
            Debug.Log("Transform position" + transform.position);
            Debug.Log("Camera position" + camera.position);
            accumulatedForce += direction.normalized * gazeRayForce;
            gazeCoolTime = 0.5f;
            Debug.Log("Force applied" + accumulatedForce);
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isStasised)
        {
            Vector3 force = collision.impulse / Time.fixedDeltaTime;
            //Scale down the force
            force /= 100;
            accumulatedForce += force * -1;
            Debug.Log("Force accumulated: " + force);
        }
    }


}