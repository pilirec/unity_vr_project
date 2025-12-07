using System.Collections.Generic;
using UnityEngine;

public class AdjacentCubeManager : MonoBehaviour
{
    [Header("Optional")]
    public Material mergedCubeMaterial; // Material for merged voxels (set of cubes)

    private GridManager gridManager;
    // Dictionary mapping grid positions to cube GameObjects.
    private Dictionary<Vector3Int, GameObject> placedCubes = new Dictionary<Vector3Int, GameObject>();

    private void Awake()
    {
        gridManager = GetComponent<GridManager>();
        if (gridManager == null)
        {
            Debug.LogError("No GridManager found on the same GameObject as AdjacentCubeManager");
        }
    }

    // Called when a new cube is placed.
    public void OnCubePlaced(Vector3Int position, GameObject cubeObject)
    {
        placedCubes[position] = cubeObject; // set the cube at position 
        UpdateCubeAndNeighbors(position); // check adjanencey for merging 
    }

    // Called when a cube is removed.
    public void OnCubeRemoved(Vector3Int position)
    {
        // remove cube if it exists 
        if (placedCubes.ContainsKey(position))
        {
            placedCubes.Remove(position);
            // Update adjacent cubes of removed cube 
            foreach (Vector3Int neighbor in GetAdjacentPositions(position))
            {
                if (placedCubes.ContainsKey(neighbor))
                {
                    UpdateCubeAppearance(neighbor);
                }
            }
        }
    }

    // Updates the appearance of the cube at the given position and its neighbors.
    // changes highlight material so user knows which voxel a part of larger voxel structure 
    private void UpdateCubeAndNeighbors(Vector3Int position)
    {
        UpdateCubeAppearance(position);
        foreach (Vector3Int neighbor in GetAdjacentPositions(position))
        {
            if (placedCubes.ContainsKey(neighbor))
            {
                UpdateCubeAppearance(neighbor);
            }
        }
    }

    // Updates the cube’s material based on adjacent cubes.
    // Checks only left, right, up, and down.
    private void UpdateCubeAppearance(Vector3Int position)
    {
        if (!placedCubes.TryGetValue(position, out GameObject cube))
            return;

        // Check for adjacent cubes (only left, right, up, and down).
        List<Vector3Int> adjacentPositions = GetAdjacentPositions(position);
        bool hasAdjacent = false;
        foreach (Vector3Int adjPos in adjacentPositions)
        {
            if (placedCubes.ContainsKey(adjPos))
            {
                hasAdjacent = true;
                break;
            }
        }

        // Change the cube’s material based on adjacency.
        MeshRenderer renderer = cube.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            if (mergedCubeMaterial != null && hasAdjacent)
            {
                renderer.material = mergedCubeMaterial;
            }
            else
            {
                // Revert to the default material provided by GridManager.
                renderer.material = gridManager.defaultMaterial;
            }
        }
    }

    // Returns a list of adjacent grid positions for left, right, up, and down.
    private List<Vector3Int> GetAdjacentPositions(Vector3Int position)
    {
        return new List<Vector3Int>
        {
            position + Vector3Int.right,
            position + Vector3Int.left,
            position + Vector3Int.up,
            position + Vector3Int.down,
        };
    }

    // Performs a flood-fill algorithm (BFS) to get all cubes connected (by left, right, up, and down) starting at the given grid position.
    // This is useful for lifting an entire merged group.
    public List<GameObject> GetConnectedCubeGroup(Vector3Int startPosition)
    {
        List<GameObject> group = new List<GameObject>();
        if (!placedCubes.ContainsKey(startPosition)) // if only 1 voxel return 
            return group;

        Queue<Vector3Int> toVisit = new Queue<Vector3Int>(); // queue to keep track of visiting voxels
        HashSet<Vector3Int> visited = new HashSet<Vector3Int>(); // set to keep track of voxels seen

        // initialize at current voxel at start position 
        toVisit.Enqueue(startPosition);
        visited.Add(startPosition);

        // loop until empty queue 
        while (toVisit.Count > 0)
        {
            Vector3Int pos = toVisit.Dequeue();
            if (placedCubes.TryGetValue(pos, out GameObject cube)) // if cube is adjancent then add to group as its a connected structure 
            {
                group.Add(cube);
            }

            // Only consider left, right, up, and down neighbors.
            foreach (Vector3Int neighbor in GetAdjacentPositions(pos))
            {
                // add voxel to queue only if voxel isn't visited and if it is part of the grid structure 
                if (!visited.Contains(neighbor) && placedCubes.ContainsKey(neighbor))
                {
                    visited.Add(neighbor);
                    toVisit.Enqueue(neighbor);
                }
            }
        }

        return group;
    }
}
