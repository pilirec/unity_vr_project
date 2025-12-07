using UnityEngine;


public class XRCustomRayVisual : MonoBehaviour
{
    [Header("Ray Settings")]
    // boolean to know if its for left or right controller
    public bool isLeftController = false;

    // The default ray colour for right controller.
    public Color rayColor = Color.blue;
    // The left controller's ray colour if isLeftController is true.
    public Color leftRayColor = Color.green;

    public float rayWidth = 0.01f;
    public bool showRayOnlyWhenHitting = false;

    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor rayInteractor;
    private LineRenderer lineRenderer;

    private void Awake()
    {
        rayInteractor = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor>();

        // Create and set up the LineRenderer if it doesn't exist.
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.startWidth = rayWidth;
            lineRenderer.endWidth = rayWidth;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));

            // Use leftRayColor if this is the left controller; otherwise, use rayColor.
            Color usedColor = isLeftController ? leftRayColor : rayColor;
            lineRenderer.startColor = usedColor;
            lineRenderer.endColor = usedColor;
            lineRenderer.positionCount = 2;
        }
    }

    private void Update()
    {
        if (rayInteractor == null || lineRenderer == null)
            return;

        bool isHitting = rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hitInfo);

        // Only show the ray when hitting something if that option is enabled.
        if (showRayOnlyWhenHitting && !isHitting)
        {
            lineRenderer.enabled = false;
            return;
        }

        lineRenderer.enabled = true;

        // Determine the ray's origin and direction.
        Vector3 rayOrigin = rayInteractor.rayOriginTransform.position;
        Vector3 rayDirection = rayInteractor.rayOriginTransform.forward;

        // Set the starting point of the ray.
        lineRenderer.SetPosition(0, rayOrigin);

        // If there's a hit, end at the hit point, otherwise use a default long distance (30).
        if (isHitting)
        {
            lineRenderer.SetPosition(1, hitInfo.point);
        }
        else
        {
            lineRenderer.SetPosition(1, rayOrigin + rayDirection * 10f);
        }
    }
}
