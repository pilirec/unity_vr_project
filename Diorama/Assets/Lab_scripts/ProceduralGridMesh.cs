// 1. Grid Mesh Generator
using UnityEngine;

public class ProceduralGridMesh : MonoBehaviour
{
    public float gridSize = 10f;  // Total grid size
    public int gridResolution = 20;  // Number of grid lines
    public Color gridColor = new Color(0.5f, 0.5f, 0.5f, 0.2f);
    public float lineThickness = 0.01f;

    void Start()
    {
        CreateGridMesh();
    }

    void CreateGridMesh()
    {
        Mesh gridMesh = new Mesh();
        GetComponent<MeshFilter>().mesh = gridMesh;

        int lines = (gridResolution + 1) * 2;
        Vector3[] vertices = new Vector3[lines * 2];
        int[] indices = new int[lines * 2];

        int v = 0;
        for (int i = 0; i <= gridResolution; i++)
        {
            float pos = (float)i / gridResolution * gridSize - gridSize / 2;

            // Vertical line
            vertices[v] = new Vector3(pos, 0, -gridSize / 2);
            vertices[v + 1] = new Vector3(pos, 0, gridSize / 2);
            indices[v] = v;
            indices[v + 1] = v + 1;
            v += 2;

            // Horizontal line
            vertices[v] = new Vector3(-gridSize / 2, 0, pos);
            vertices[v + 1] = new Vector3(gridSize / 2, 0, pos);
            indices[v] = v;
            indices[v + 1] = v + 1;
            v += 2;
        }

        gridMesh.vertices = vertices;
        gridMesh.SetIndices(indices, MeshTopology.Lines, 0);
        gridMesh.RecalculateBounds();

        MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.material = new Material(Shader.Find("Unlit/Color"));
        renderer.material.color = gridColor;
    }
}