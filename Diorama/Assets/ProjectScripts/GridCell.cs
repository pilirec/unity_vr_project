using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    [HideInInspector] public Vector3Int gridPosition; // position of grid base
    [HideInInspector] public GridManager gridManager; // grid manager game object

    private MeshRenderer meshRenderer; // mesh renderer of grid cell for material
    private Material originalMaterial; // original material of grid cell
    private Material highlightMaterial; // highlight material when controller is pointed at grid cell


    public void Initialize(Vector3Int position, GridManager manager)
    {
        // initialize position and manager on load
        gridPosition = position;
        gridManager = manager;

        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            // Store the original material and highlight material
            originalMaterial = meshRenderer.material;
            highlightMaterial = gridManager.highlightMaterial;

            // Make the grid cell invisible by default so that only highlight texture renders when ray is pointing at grid cell
            meshRenderer.enabled = false;
        }
    }

    // function to highlight grid cells
    public void Highlight(bool isHighlighted)
    {
        if (meshRenderer != null)
        {
            // if the current cell is highlighted then set the material to highlight material and make the voxel visible. 
            if (isHighlighted)
            {
                meshRenderer.material = highlightMaterial;
                meshRenderer.enabled = true;
            }

            // otherwise set material to original material
            else
            {
                meshRenderer.material = originalMaterial;
                // only hide the grid cell if there is no voxel at that grid cell position 
                if (!gridManager.HasCube(gridPosition))
                {
                    meshRenderer.enabled = false;
                }
            }
        }
    }

    // helper function to change visiblity when placing and removing voxels 
    public void UpdateVisibility(bool hasCube)
    {
        if (meshRenderer != null)
        {
            meshRenderer.enabled = hasCube; // visible only when a voxel exists at grid cell
        }
    }
}