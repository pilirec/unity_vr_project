using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


// This script is intended for the left controller. It allows the user to grab a cube or a connected group of cubes
// (as determined by AdjacentCubeManager) using the trigger button. While held, the group follows the controller's movement,
// and the grip button can be used to uniformly scale the group. Upon release, the cubes are snapped back to grid positions. If the user is near the grid
// otherwise, the voxel structure lands on the ground. 
public class CubeGrabber : MonoBehaviour
{
    [Header("References")]
    public GridManager gridManager; // grid manager script
    public AdjacentCubeManager adjacentCubeManager; // adjacent cube manager script 

    [Header("Input Actions")]
    public InputActionProperty grabAction;   // Trigger button for grab
    public InputActionProperty scaleAction;  // Grip button for scaling

    [Header("Scaling Settings")]
    public float scalingSpeed = 3.0f;     // scale speed
    public float minScale = 0.5f;         // Minimum allowed scale
    public float maxScale = 4.0f;         // Maximum allowed scale
    private float currentScaleFactor = 1.0f;

    [Header("Proximity Settings")]
    public float gridSnapDistance = 30.0f;   // Distance threshold for snapping to grid
    public float groundPlaneY = -4.0f;       // Y position of the ground plane
    public float fallingSpeed = 9.8f;       // Speed at which objects fall to ground (gravity)
    public LayerMask groundLayerMask;       // Layer mask for ground raycast detection (default layer)

    private GridCell lastHighlightedCell;

    // Reference to the XRRayInteractor used for hitting cubes.
    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor rayInteractor;

    // Held group container: when cubes are grabbed, they are reparented under this object.
    private GameObject heldGroup;

    // List to store original grid positions (if needed for snapping back).
    private Dictionary<GameObject, Vector3Int> originalGridPositions = new Dictionary<GameObject, Vector3Int>();

    // Flag to track if currently holding a group.
    private bool isHolding = false;

    // Original local uniform scale for when cubes are in the grid.
    private float originalScale = 1f;

    // Debugging
    [Header("Debug")]
    public bool enableDebugLogs = true;

    private void Awake()
    {
        // Get the ray interactor from the same GameObject.
        rayInteractor = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor>();
        if (rayInteractor == null)
        {
            Debug.LogError("No XRRayInteractor found on " + gameObject.name);
        }

        // Create an empty holder for the grabbed cubes.
        heldGroup = new GameObject("HeldCubeGroup");

        // Make heldGroup follow controller.
        heldGroup.transform.parent = transform;

        if (gridManager == null)
        {
            Debug.LogError("GridManager reference is missing on " + gameObject.name);
        }

        if (adjacentCubeManager == null)
        {
            Debug.LogError("AdjacentCubeManager reference is missing on " + gameObject.name);
        }

        if (grabAction.action == null)
        {
            Debug.LogError("Grab action is not assigned on " + gameObject.name);
        }

        if (scaleAction.action == null)
        {
            Debug.LogError("Scale action is not assigned on " + gameObject.name);
        }

        DebugLog("CubeGrabber initialized");
    }

    private void OnEnable()
    {
        // apply grab action if voxel group grabbed
        if (grabAction.action != null)
        {
            grabAction.action.Enable();
            grabAction.action.performed += OnGrabPerformed;
            grabAction.action.canceled += OnGrabReleased;
            DebugLog("Grab action enabled: " + grabAction.action.name);
        }

        // apply scaling if voxel shape is being held 
        if (scaleAction.action != null)
        {
            scaleAction.action.Enable();
            scaleAction.action.performed += OnScalePerformed;
            DebugLog("Scale action enabled: " + scaleAction.action.name);
        }
    }

    private void OnDisable()
    {
        // Disable grab and drop voxel shape (snap or drop depends on distance to grid)
        if (grabAction.action != null)
        {
            grabAction.action.Disable();
            grabAction.action.performed -= OnGrabPerformed;
            grabAction.action.canceled -= OnGrabReleased;
        }

        // Disable and stop scaling (scale reset only on snap)
        if (scaleAction.action != null)
        {
            scaleAction.action.Disable();
            scaleAction.action.performed -= OnScalePerformed;
        }

        // Unhighlight the last cell when disabled
        if (lastHighlightedCell != null)
        {
            gridManager.UnhighlightCell(lastHighlightedCell.gridPosition);
            lastHighlightedCell = null;
        }
    }

    // contineously update the highlighting of grid cells
    private void Update()
    {
        if (rayInteractor != null)
        {
            UpdateRaycastHighlighting();
        }
    }

    private void UpdateRaycastHighlighting()
    {
        // Unhighlight the previous cell
        if (lastHighlightedCell != null)
        {
            gridManager.UnhighlightCell(lastHighlightedCell.gridPosition);
            lastHighlightedCell = null;
        }

        // Check if we're hitting anything with the ray
        if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hitInfo))
        {
            // Check if we hit a grid cell
            GridCell hitCell = hitInfo.collider.GetComponent<GridCell>();
            if (hitCell != null)
            {
                // Highlight the cell
                gridManager.HighlightCell(hitCell.gridPosition);
                lastHighlightedCell = hitCell;
            }
        }
    }

    // Called when the grab (trigger) is pressed.
    // Casts a ray to see if a cube is hit, and if so, grabs the entire connected group.
    private void OnGrabPerformed(InputAction.CallbackContext context)
    {
        DebugLog("OnGrabPerformed called - value: " + context.ReadValueAsButton());

        if (isHolding)
        {
            DebugLog("Already holding a group, ignoring grab action");
            return;
        }

        if (rayInteractor == null || gridManager == null || adjacentCubeManager == null)
        {
            Debug.LogError("Required references missing. Cannot grab cubes.");
            return;
        }

        // Use the ray interactor to see if we hit something
        if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hitInfo))
        {
            GameObject hitObject = hitInfo.collider.gameObject;
            DebugLog("Raycast hit: " + hitObject.name);

            Vector3Int? gridPos = null;

            // Check if we hit a Cube directly
            if (hitObject.name.StartsWith("Cube_"))
            {
                DebugLog("Hit a cube directly: " + hitObject.name);
                gridPos = ParseGridPosition(hitObject.name);
            }
            // Check if we hit a GridCell that has a cube
            else if (hitObject.name.StartsWith("GridCell_"))
            {
                DebugLog("Hit a grid cell: " + hitObject.name);

                // Parse grid position from the GridCell name
                gridPos = ParseGridPosition(hitObject.name.Replace("GridCell_", "Cube_"));

                // Check if there's a cube at this position
                if (gridPos.HasValue && !gridManager.HasCube(gridPos.Value))
                {
                    DebugLog("No cube at grid position: " + gridPos.Value);
                    gridPos = null;
                }
                else
                {
                    DebugLog("Found cube at grid position: " + gridPos.Value);
                }
            }

            // If we found a valid grid position with a cube, grab the connected group
            if (gridPos.HasValue)
            {
                // Get all connected cubes from the AdjacentCubeManager
                List<GameObject> group = adjacentCubeManager.GetConnectedCubeGroup(gridPos.Value);
                DebugLog("Found " + group.Count + " connected cubes");

                if (group.Count > 0)
                {
                    // Mark as holding and store original grid positions
                    isHolding = true;
                    originalGridPositions.Clear();

                    foreach (GameObject cube in group)
                    {
                        if (cube == null)
                        {
                            DebugLog("Skipping null cube in connected group");
                            continue;
                        }

                        // Extract the original grid position from the cube's name
                        Vector3Int pos = ParseGridPosition(cube.name);
                        originalGridPositions[cube] = pos;

                        // Don't destroy the cube, just remove it from management
                        gridManager.UnregisterCube(pos);

                        // Reparent the cube to the heldGroup
                        cube.transform.parent = heldGroup.transform;
                        DebugLog("Grabbed cube at position: " + pos);
                    }

                    // Store the original scale
                    if (group[0] != null)
                    {
                        originalScale = group[0].transform.localScale.x;
                    }
                    DebugLog("Successfully grabbed group of " + originalGridPositions.Count + " cubes");
                }
            }
            else
            {
                DebugLog("Could not determine a valid grid position with a cube");
            }
        }
        else
        {
            DebugLog("Raycast didn't hit anything");
        }
    }

    // Called when the grab (trigger) is released with distance-based behavior.
    private void OnGrabReleased(InputAction.CallbackContext context)
    {
        DebugLog("OnGrabReleased called");

        if (!isHolding)
        {
            DebugLog("Not holding anything, ignoring release action");
            return;
        }

        if (gridManager == null)
        {
            Debug.LogError("GridManager reference missing. Cannot release cubes properly.");
            isHolding = false;
            originalGridPositions.Clear();
            return;
        }

        DebugLog("Releasing " + originalGridPositions.Count + " cubes");

        // Temp list of cubes 
        List<KeyValuePair<GameObject, Vector3Int>> validCubes = new List<KeyValuePair<GameObject, Vector3Int>>();

        // First check which cubes are still valid (not destroyed)
        foreach (KeyValuePair<GameObject, Vector3Int> pair in originalGridPositions)
        {
            GameObject cube = pair.Key;
            if (cube != null)
            {
                validCubes.Add(pair);
            }
            else
            {
                DebugLog("Skipping destroyed cube");
            }
        }

        // Reset the scale factor for next time
        currentScaleFactor = 1.0f;

        // Check distance between held group and grid center to determine if we should snap or drop
        bool shouldSnapToGrid = IsCloseToGrid();

        if (shouldSnapToGrid)
        {
            DebugLog("Close to grid - snapping cubes to grid positions");

            // Process cubes for grid snapping 
            foreach (KeyValuePair<GameObject, Vector3Int> pair in validCubes)
            {
                GameObject cube = pair.Key;
                Vector3Int gridPos = pair.Value;

                if (cube != null)
                {
                    try
                    {
                        // Reparent the cube back to the grid manager's transform
                        cube.transform.parent = gridManager.transform;

                        // Reset the scale
                        cube.transform.localScale = Vector3.one * originalScale;

                        // Reset the rotation to default (aligned with grid cells)
                        cube.transform.rotation = Quaternion.identity;

                        // Position the cube correctly at its grid position
                        Vector3 worldPos = gridManager.GetWorldPosition(gridPos);
                        if (worldPos != Vector3.zero)
                        {
                            cube.transform.position = worldPos;
                        }

                        // Register the cube with the grid manager
                        gridManager.RegisterExistingCube(gridPos, cube);

                        DebugLog("Released cube back to position: " + gridPos);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError("Error releasing cube: " + e.Message);
                    }
                }
            }
        }
        else
        {
            DebugLog("Far from grid - dropping cubes to ground");

            // Create a parent for the dropped objects
            GameObject droppedGroup = new GameObject("DroppedCubeGroup");
            droppedGroup.transform.position = heldGroup.transform.position;

            // Maintain the current rotation
            droppedGroup.transform.rotation = heldGroup.transform.rotation;

            // Process cubes for dropping to ground
            foreach (KeyValuePair<GameObject, Vector3Int> pair in validCubes)
            {
                GameObject cube = pair.Key;

                if (cube != null)
                {
                    try
                    {
                        // Keep the current rotation and scale
                        // Just reparent to our new dropped group
                        cube.transform.parent = droppedGroup.transform;

                        DebugLog("Dropped cube to ground");

                        // Add a rigidbody for physics simulation 
                        if (cube.GetComponent<Rigidbody>() == null)
                        {
                            Rigidbody rb = cube.AddComponent<Rigidbody>();
                            rb.mass = 2.0f;
                            rb.linearDamping = 0.3f;
                            rb.useGravity = true;
                        }
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError("Error dropping cube: " + e.Message);
                    }
                }
            }

            // Add a rigidbody to the voxel shape itself
            Rigidbody groupRb = droppedGroup.AddComponent<Rigidbody>();
            groupRb.useGravity = true;

            // Drop the object directly to the ground
            StartCoroutine(DropToGround(droppedGroup));
        }

        // Clear the voxel shape container
        if (heldGroup != null)
        {
            heldGroup.transform.localPosition = Vector3.zero;
            heldGroup.transform.localScale = Vector3.one;
        }

        isHolding = false;
        originalGridPositions.Clear();
        DebugLog("Successfully released group");
    }

    // Determines if the voxel shape is close enough to the grid to snap
    private bool IsCloseToGrid()
    {
        if (heldGroup == null || gridManager == null)
            return false;

        // Calculate distance between voxels and grid center
        Vector3 gridCenter = gridManager.transform.position;
        float distance = Vector3.Distance(heldGroup.transform.position, gridCenter);

        DebugLog("Distance to grid: " + distance + " (threshold: " + gridSnapDistance + ")");

        // Return true if close enough to snap
        return distance <= gridSnapDistance;
    }

    // helper function to animate falling
    private IEnumerator<object> DropToGround(GameObject droppedObject)
    {
        if (droppedObject == null)
            yield break;

        // Get the current position
        Vector3 startPos = droppedObject.transform.position;

        // Calculate target position on ground
        Vector3 targetPos = startPos;

        // Try to raycast to find actual ground position
        RaycastHit hit;
        if (Physics.Raycast(startPos, Vector3.down, out hit, 100f, groundLayerMask))
        {
            targetPos.y = hit.point.y + 0.1f; 
        }
        else
        {
            // Default ground plane if no raycast hit
            targetPos.y = groundPlaneY;
        }

        // Animate falling
        float distance = startPos.y - targetPos.y;
        float duration = Mathf.Sqrt(2 * distance / fallingSpeed); // Physics-based time calculation (kinematic equations)
        float elapsed = 0f;

        while (elapsed < duration && droppedObject != null)
        {
            // Simple falling animation
            float t = elapsed / duration;
            float newY = Mathf.Lerp(startPos.y, targetPos.y, t * t); // Quadratic easing for physics-like fall (similar to projectile motion)

            if (droppedObject != null) // if object is null simply transform to ground position 
            {
                Vector3 newPos = droppedObject.transform.position;
                newPos.y = newY;
                droppedObject.transform.position = newPos;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure we reach the final position
        if (droppedObject != null)
        {
            Vector3 finalPos = droppedObject.transform.position;
            finalPos.y = targetPos.y;
            droppedObject.transform.position = finalPos;

            // Let Unity handle physics motion after animation 
            Rigidbody[] rigidbodies = droppedObject.GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody rb in rigidbodies)
            {
                rb.isKinematic = false;
            }
        }
    }

    // Called when the grip (scale) action is performed.
    // Adjusts the scale of the voxel shape uniformly.
    private void OnScalePerformed(InputAction.CallbackContext context)
    {
        float scaleInput = context.ReadValue<float>(); // get current scale 
        DebugLog("OnScalePerformed called with value: " + scaleInput);

        if (!isHolding)
        {
            DebugLog("Not holding anything, ignoring scale action");
            return;
        }

        // Use the scaling speed multiplier to visually see scaling
        // Apply the scaling to our tracking factor
        currentScaleFactor += scaleInput * Time.deltaTime * scalingSpeed;

        // Clamp the scale within reasonable bounds
        currentScaleFactor = Mathf.Clamp(currentScaleFactor, minScale, maxScale);

        // Apply the new scale to the voxels 
        float newScale = originalScale * currentScaleFactor;
        heldGroup.transform.localScale = Vector3.one * newScale;

        DebugLog("Scaled group to: " + newScale + " (factor: " + currentScaleFactor + ")");
    }

    // Parses a grid position from a cube name.
    // cube name format: "Cube_x_y_z"
    private Vector3Int ParseGridPosition(string cubeName)
    {
        // extracts x y z pos from cube key name
        string[] parts = cubeName.Split('_');
        if (parts.Length >= 4)
        {
            try
            {
                int x = int.Parse(parts[1]);
                int y = int.Parse(parts[2]);
                int z = int.Parse(parts[3]);
                return new Vector3Int(x, y, z);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error parsing grid position from cube name: " + cubeName + " - " + e.Message);
            }
        }
        else
        {
            Debug.LogError("Invalid cube name format: " + cubeName);
        }
        return Vector3Int.zero;
    }

    private void DebugLog(string message)
    {
        if (enableDebugLogs)
        {
            Debug.Log("[CubeGrabber] " + message);
        }
    }
}