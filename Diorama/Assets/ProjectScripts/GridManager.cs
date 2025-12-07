using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public int gridSizeX = 10; // size of grid in x dir
    public int gridSizeY = 10; // size of grid in y dir
    public int gridSizeZ = 10; // size of grid in z dir
    public float cellSize = 1.5f; // size of cell
    public float gridSpacing = 0.0f; // Base spacing between grid cells

    [Header("Prefabs")]
    public GameObject gridCellPrefab; // asset for grid cell
    public GameObject cubePrefab; // asset for voxel

    [Header("Materials")]
    public Material defaultMaterial; // default material (no connectons)
    public Material highlightMaterial; // highlight material (ray or adjacent voxels exists)

    // Dictionary to store grid cells and their world positions
    private Dictionary<Vector3Int, GridCell> gridCells = new Dictionary<Vector3Int, GridCell>();

    // Dictionary to track placed cubes
    private Dictionary<Vector3Int, GameObject> placedCubes = new Dictionary<Vector3Int, GameObject>();

    // Reference to our adjacency manager
    private AdjacentCubeManager adjacencyManager;

    private void Awake()
    {
        // Get or add the AdjacentCubeManager component
        adjacencyManager = GetComponent<AdjacentCubeManager>();
        if (adjacencyManager == null)
        {
            adjacencyManager = gameObject.AddComponent<AdjacentCubeManager>();
        }
    }

    private void Start()
    {
        CreateGrid();
    }

    // Accessor methods
    public float GetCellSize() { return cellSize; }
    public float GetGridSpacing() { return gridSpacing; }

    private void CreateGrid()
    {
        float actualCellSize = cellSize + gridSpacing;

        // Calculate grid center offset for centering the grid
        Vector3 centerOffset = new Vector3(
            (gridSizeX * actualCellSize) / 2 - actualCellSize / 2,
            (gridSizeY * actualCellSize) / 2 - actualCellSize / 2,
            (gridSizeZ * actualCellSize) / 2 - actualCellSize / 2
        );

        // Create the grid
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                for (int z = 0; z < gridSizeZ; z++)
                {
                    Vector3Int gridPosition = new Vector3Int(x, y, z);

                    // Calculate world position with spacing
                    Vector3 worldPosition = new Vector3(
                        x * actualCellSize - centerOffset.x,
                        y * actualCellSize - centerOffset.y,
                        z * actualCellSize - centerOffset.z
                    );

                    // Instantiate grid cell
                    GameObject cellObject = Instantiate(gridCellPrefab, worldPosition, Quaternion.identity, transform);
                    cellObject.name = $"GridCell_{x}_{y}_{z}";

                    // Resize the cell to match our cell size
                    cellObject.transform.localScale = Vector3.one * cellSize;

                    // Add GridCell component
                    GridCell gridCell = cellObject.GetComponent<GridCell>();
                    if (gridCell == null)
                    {
                        gridCell = cellObject.AddComponent<GridCell>();
                    }

                    // Set up grid cell properties
                    gridCell.Initialize(gridPosition, this);

                    // Add to dictionary
                    gridCells.Add(gridPosition, gridCell);
                }
            }
        }
    }

    public void HighlightCell(Vector3Int gridPosition)
    {
        // highlight the grid cell if no cube is there or it is part of a larger voxel structure
        if (gridCells.TryGetValue(gridPosition, out GridCell cell))
        {
            cell.Highlight(true);
        }
    }

    public void UnhighlightCell(Vector3Int gridPosition)
    {
        // unhighlight grid cell if independent voxel exists 
        if (gridCells.TryGetValue(gridPosition, out GridCell cell))
        {
            cell.Highlight(false);
        }
    }

    public void PlaceCube(Vector3Int gridPosition)
    {
        // Check if there's already a cube at this position
        if (placedCubes.ContainsKey(gridPosition))
        {
            return;
        }

        if (gridCells.TryGetValue(gridPosition, out GridCell cell))
        {
            // Create a new cube at the grid cell position (initial position, will be adjusted)
            GameObject cube = Instantiate(cubePrefab, cell.transform.position, Quaternion.identity, transform);
            cube.name = $"Cube_{gridPosition.x}_{gridPosition.y}_{gridPosition.z}";
            cube.transform.localScale = Vector3.one * cellSize;

            // Add to dictionary
            placedCubes.Add(gridPosition, cube);

            // Update cell visibility
            cell.UpdateVisibility(true);

            // Notify adjacency manager about the new cube
            adjacencyManager.OnCubePlaced(gridPosition, cube);
        }
    }

    public void RemoveCube(Vector3Int gridPosition)
    {
        if (placedCubes.TryGetValue(gridPosition, out GameObject cube))
        {
            // Notify adjacency manager before destroying the cube
            adjacencyManager.OnCubeRemoved(gridPosition);

            // Now handle the cube removal
            placedCubes.Remove(gridPosition);

            // Update cell visibility - set to false since cube is removed
            if (gridCells.TryGetValue(gridPosition, out GridCell cell))
            {
                cell.UpdateVisibility(false);
            }

            // Only destroy the GameObject if it's still present and active
            if (cube != null)
            {
                Destroy(cube);
            }
        }
    }

    public bool HasCube(Vector3Int gridPosition)
    {
        return placedCubes.ContainsKey(gridPosition);
    }

    // Helper method to get the GameObject for a cube at a specific position
    public bool GetCubeGameObject(Vector3Int position, out GameObject cube)
    {
        return placedCubes.TryGetValue(position, out cube);
    }

    //  Converts a grid position to a world position
    public Vector3 GetWorldPosition(Vector3Int gridPosition)
    {
        if (gridCells.TryGetValue(gridPosition, out GridCell cell))
        {
            return cell.transform.position;
        }

        // Calculate position manually if the grid cell doesn't exist
        float actualCellSize = cellSize + gridSpacing;

        // Calculate grid center offset for centering
        Vector3 centerOffset = new Vector3(
            (gridSizeX * actualCellSize) / 2 - actualCellSize / 2,
            (gridSizeY * actualCellSize) / 2 - actualCellSize / 2,
            (gridSizeZ * actualCellSize) / 2 - actualCellSize / 2
        );

        // Calculate world position with spacing
        return new Vector3(
            gridPosition.x * actualCellSize - centerOffset.x,
            gridPosition.y * actualCellSize - centerOffset.y,
            gridPosition.z * actualCellSize - centerOffset.z
        );
    }

    // Registers an existing cube at the given grid position without creating a new one
    public void RegisterExistingCube(Vector3Int gridPosition, GameObject cube)
    {
        // Check if there's already a cube at this position
        if (placedCubes.ContainsKey(gridPosition))
        {
            // If it's the same cube, nothing to do
            if (placedCubes[gridPosition] == cube)
                return;

            // Otherwise, remove the old one
            RemoveCube(gridPosition);
        }

        // Add to dictionary without instantiating a new cube
        placedCubes.Add(gridPosition, cube);

        // Update cell visibility
        if (gridCells.TryGetValue(gridPosition, out GridCell cell))
        {
            cell.UpdateVisibility(true);
        }

        // Notify adjacency manager about the cube
        AdjacentCubeManager adjacencyManager = GetComponent<AdjacentCubeManager>();
        if (adjacencyManager != null)
        {
            adjacencyManager.OnCubePlaced(gridPosition, cube);
        }
    }

    // Removes a cube from management without destroying the GameObject
    public void UnregisterCube(Vector3Int gridPosition)
    {
        if (placedCubes.TryGetValue(gridPosition, out GameObject _))
        {
            // Notify adjacency manager before removing from dictionary
            AdjacentCubeManager adjacencyManager = GetComponent<AdjacentCubeManager>();
            if (adjacencyManager != null)
            {
                adjacencyManager.OnCubeRemoved(gridPosition);
            }

            // Remove from dictionary without destroying
            placedCubes.Remove(gridPosition);

            // Update cell visibility
            if (gridCells.TryGetValue(gridPosition, out GridCell cell))
            {
                cell.UpdateVisibility(false);
            }
        }
    }
}