using System.Collections.Generic;
using UnityEngine;

public class SelectableObjectController : MonoBehaviour
{
    [Tooltip("The material used to highlight the selected object.")]
    public Material highlightMaterial;

    [Tooltip("Range for random position when instantiating a new object.")]
    public float randomRange = 10f;

    // List of selectable objects (populated at Start by finding objects tagged "Selectable")
    private List<GameObject> selectableObjects;
    // Index of the currently selected object in the list
    private int currentIndex = 0;
    // Save each object's original materials so we can restore them when unhighlighting
    private Dictionary<GameObject, Material[]> originalMaterials = new Dictionary<GameObject, Material[]>();

    void Start()
    {
        // Find all objects in the scene tagged as "Selectable"
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Selectable");
        selectableObjects = new List<GameObject>(objs);

        // For each object, store its original materials (if it has a Renderer)
        foreach (GameObject obj in selectableObjects)
        {
            Renderer rend = obj.GetComponent<Renderer>();
            if (rend != null)
            {
                // Save a copy of the current materials (note: materials is an array)
                originalMaterials[obj] = rend.materials;
            }
        }

        // If at least one object is found, highlight the first one
        if (selectableObjects.Count > 0)
        {
            HighlightObject(selectableObjects[currentIndex]);
        }
    }

    void Update()
    {
        // Press Z to cycle to the next selectable object.
        if (Input.GetKeyDown(KeyCode.Z))
        {
            CycleSelection();
        }

        // Press P to destroy/remove the currently selected object.
        if (Input.GetKeyDown(KeyCode.X))
        {
            RemoveSelectedObject();
        }

        // Press O to instantiate (clone) a new copy of the currently selected object.
        if (Input.GetKeyDown(KeyCode.C))
        {
            InstantiateNewObject();
        }
    }

    // Cycles selection to the next object in the list.
    void CycleSelection()
    {
        if (selectableObjects.Count == 0)
            return;

        // Unhighlight current object.
        UnhighlightObject(selectableObjects[currentIndex]);

        // Cycle to next object (wrap around if needed)
        currentIndex = (currentIndex + 1) % selectableObjects.Count;

        // Highlight the new selected object.
        HighlightObject(selectableObjects[currentIndex]);
    }

    // Removes (destroys) the currently selected object.
    void RemoveSelectedObject()
    {
        if (selectableObjects.Count == 0)
            return;

        GameObject objToRemove = selectableObjects[currentIndex];

        // Remove the object from our list and dictionary.
        selectableObjects.RemoveAt(currentIndex);
        originalMaterials.Remove(objToRemove);

        // Destroy the object.
        Destroy(objToRemove);

        // If objects remain, adjust currentIndex and highlight the new selection.
        if (selectableObjects.Count > 0)
        {
            currentIndex = currentIndex % selectableObjects.Count;
            HighlightObject(selectableObjects[currentIndex]);
        }
    }

    // Instantiates a new copy of the currently selected object and places it at a random position.
    void InstantiateNewObject()
    {
        if (selectableObjects.Count == 0)
            return;

        GameObject selected = selectableObjects[currentIndex];
        // Create a new copy of the selected object.
        GameObject newObj = Instantiate(selected);

        // Assign a random position within the specified range.
        newObj.transform.position = new Vector3(
            Random.Range(-randomRange, randomRange),
            Random.Range(0, randomRange),
            Random.Range(-randomRange, randomRange)
        );

        // Ensure the new object has the "Selectable" tag.
        newObj.tag = "Selectable";

        // Save its original materials.
        Renderer rend = newObj.GetComponent<Renderer>();
        if (rend != null)
        {
            originalMaterials[newObj] = rend.materials;
        }

        // Add the new object to the list of selectable objects.
        selectableObjects.Add(newObj);

        // (Optional) Switch selection to the new object:
        UnhighlightObject(selected);
        currentIndex = selectableObjects.Count - 1;
        HighlightObject(newObj);
    }

    // Highlights an object by replacing all its materials with the highlight material.
    void HighlightObject(GameObject obj)
    {
        Renderer rend = obj.GetComponent<Renderer>();
        if (rend != null && highlightMaterial != null)
        {
            int count = rend.materials.Length;
            Material[] highlightMats = new Material[count];
            for (int i = 0; i < count; i++)
            {
                highlightMats[i] = highlightMaterial;
            }
            rend.materials = highlightMats;
        }
    }

    // Restores the object's original materials to remove the highlight.
    void UnhighlightObject(GameObject obj)
    {
        Renderer rend = obj.GetComponent<Renderer>();
        if (rend != null && originalMaterials.ContainsKey(obj))
        {
            rend.materials = originalMaterials[obj];
        }
    }
}
