using UnityEngine;

public class GazeRayRenderer : MonoBehaviour
{
    public Transform headTransform;  // camera
    public float maxRayDistance = 10f;  
    public LineRenderer lineRenderer;
    public LayerMask hitLayers;  
    void Start()
    {
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        // config LineRenderer
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.005f;
        lineRenderer.positionCount = 2;

        // set color
        lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
        lineRenderer.material.color = Color.yellow;
    }

    void Update()
    {
        if (headTransform == null)
        {
            Debug.LogError("请在 Inspector 里指定 Head Transform（通常是 Main Camera）！");
            return;
        }

        Vector3 startPoint = headTransform.position;
        Vector3 direction = headTransform.forward;

        RaycastHit hit;
        Vector3 endPoint = startPoint + direction * maxRayDistance;

        if (Physics.Raycast(startPoint, direction, out hit, maxRayDistance, hitLayers))
        {
            endPoint = hit.point;  
        }

        lineRenderer.SetPosition(0, startPoint);
        lineRenderer.SetPosition(1, endPoint);
    }

    public void StayObject()
    {
        lineRenderer.material.color = Color.red;
    }

    public void LeaveObject()
    {
        lineRenderer.material.color = Color.green;
    }


    public void Trigger()
    {

    }
}