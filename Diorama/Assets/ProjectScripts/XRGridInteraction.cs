using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

public class XRGridInteraction : MonoBehaviour
{
    [Header("References")]
    public GridManager gridManager; // grid manager script

    [Header("Actions")]
    public InputActionProperty placeCubeAction; // right trigger to add voxels 
    public InputActionProperty removeCubeAction; // right grip to remove voxel

    // Reference to the ray interactor (will find automatically if not assigned)
    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor rayInteractor;

    // Last highlighted grid cell
    private GridCell lastHighlightedCell;

    private void Awake()
    {
        // Try to get the ray interactor if not assigned
        if (rayInteractor == null)
        {
            rayInteractor = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor>();
        }

        if (rayInteractor == null)
        {
            Debug.LogError("No XR Ray Interactor found on " + gameObject.name);
        }
    }

    private void OnEnable()
    {
        // Enable actions and register callbacks
        if (placeCubeAction.action != null)
        {
            placeCubeAction.action.Enable();
            placeCubeAction.action.performed += OnPlaceCube;
        }

        if (removeCubeAction.action != null)
        {
            removeCubeAction.action.Enable();
            removeCubeAction.action.performed += OnRemoveCube;
        }
    }

    private void OnDisable()
    {
        // Disable actions and unregister callbacks
        if (placeCubeAction.action != null)
        {
            placeCubeAction.action.Disable();
            placeCubeAction.action.performed -= OnPlaceCube;
        }

        if (removeCubeAction.action != null)
        {
            removeCubeAction.action.Disable();
            removeCubeAction.action.performed -= OnRemoveCube;
        }

        // Unhighlight the last cell when disabled
        if (lastHighlightedCell != null)
        {
            gridManager.UnhighlightCell(lastHighlightedCell.gridPosition);
            lastHighlightedCell = null;
        }
    }

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

    private void OnPlaceCube(InputAction.CallbackContext context)
    {
        if (lastHighlightedCell != null)
        {
            // Check if there's already a cube at this position
            if (!gridManager.HasCube(lastHighlightedCell.gridPosition))
            {
                gridManager.PlaceCube(lastHighlightedCell.gridPosition);
            }
        }
    }

    private void OnRemoveCube(InputAction.CallbackContext context)
    {
        if (lastHighlightedCell != null)
        {
            // Only remove if there's a cube at this position
            if (gridManager.HasCube(lastHighlightedCell.gridPosition))
            {
                gridManager.RemoveCube(lastHighlightedCell.gridPosition);
            }
        }
    }
}