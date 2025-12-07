using UnityEngine;
using System.Collections.Generic;

public class IteratorScript : MonoBehaviour
{
    public List<GameObject> gameObjectsList = new List<GameObject>(); // List to store GameObjects
    public Material highlightMaterial;  // Material to apply when selected
    public float duplicateRadius = 3f;  // Radius for new object placement
    private int currentIndex = 0;       // Iterator index
    private Dictionary<GameObject, Material> originalMaterials = new Dictionary<GameObject, Material>(); // Store original materials

    void Start()
    {
        // Automatically find 5 GameObjects in the scene
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        for (int i = 0; i < Mathf.Min(5, allObjects.Length); i++)
        {
            if (allObjects[i].GetComponent<Renderer>()) // Ensure object has a Renderer
            {
                gameObjectsList.Add(allObjects[i]);

                // Store the original material
                originalMaterials[allObjects[i]] = allObjects[i].GetComponent<Renderer>().material;
            }
        }

        Debug.Log("Stored GameObjects:");
        foreach (GameObject obj in gameObjectsList)
        {
            Debug.Log(obj.name);
        }

        // Apply highlight to the first object if available
        if (gameObjectsList.Count > 0)
        {
            ApplyHighlight(gameObjectsList[currentIndex]);
        }
    }

    void Update()
    {
        if (gameObjectsList.Count == 0)
        {
            Debug.Log("No more objects to cycle through.");
            return;
        }

        // Iterate through the list when pressing S
        if (Input.GetKeyDown(KeyCode.S))
        {
            // Restore previous object's material before switching
            RestoreOriginalMaterial(gameObjectsList[currentIndex]);

            // Move to the next object
            currentIndex = (currentIndex + 1) % gameObjectsList.Count;

            // Apply highlight to the new selected object
            ApplyHighlight(gameObjectsList[currentIndex]);

            Debug.Log("Current Object: " + gameObjectsList[currentIndex].name);
        }

        // Delete the selected object when pressing K
        if (Input.GetKeyDown(KeyCode.K))
        {
            DeleteCurrentObject();
        }

        // Duplicate the selected object when pressing N
        if (Input.GetKeyDown(KeyCode.N))
        {
            DuplicateCurrentObject();
        }
    }

    void ApplyHighlight(GameObject obj)
    {
        if (obj != null && obj.GetComponent<Renderer>() && highlightMaterial != null)
        {
            obj.GetComponent<Renderer>().material = highlightMaterial;
        }
    }

    void RestoreOriginalMaterial(GameObject obj)
    {
        if (obj != null && obj.GetComponent<Renderer>() && originalMaterials.ContainsKey(obj))
        {
            obj.GetComponent<Renderer>().material = originalMaterials[obj];
        }
    }

    void DeleteCurrentObject()
    {
        if (gameObjectsList.Count == 0) return;

        GameObject objToDelete = gameObjectsList[currentIndex];

        // Remove from the list and dictionary
        originalMaterials.Remove(objToDelete);
        gameObjectsList.RemoveAt(currentIndex);

        // Destroy the object
        Destroy(objToDelete);
        Debug.Log("Deleted: " + objToDelete.name);

        // Adjust index if list isn't empty
        if (gameObjectsList.Count > 0)
        {
            currentIndex = currentIndex % gameObjectsList.Count;
            ApplyHighlight(gameObjectsList[currentIndex]);
        }
        else
        {
            Debug.Log("All objects deleted.");
        }
    }

    void DuplicateCurrentObject()
    {
        if (gameObjectsList.Count == 0) return;

        GameObject selectedObject = gameObjectsList[currentIndex];

        // Generate a random position within a sphere of radius `duplicateRadius`
        Vector3 randomOffset = Random.insideUnitSphere * duplicateRadius;
        randomOffset.y = 0; // Keep it on the same ground level

        Vector3 newPosition = selectedObject.transform.position + randomOffset;

        // Create a new copy of the selected object
        GameObject duplicate = Instantiate(selectedObject, newPosition, selectedObject.transform.rotation);
        duplicate.name = selectedObject.name + "_Clone";

        // Store the original material of the new object
        originalMaterials[duplicate] = duplicate.GetComponent<Renderer>().material;

        // Add to the list and make it the current selection
        gameObjectsList.Add(duplicate);
        currentIndex = gameObjectsList.Count - 1;

        // Apply highlight to the new object
        ApplyHighlight(duplicate);

        Debug.Log("Duplicated: " + duplicate.name + " at " + newPosition);
    }
}

