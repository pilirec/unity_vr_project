using UnityEngine;

public class ChangeColor : MonoBehaviour
{
    public Color touch;
    public bool changeBack = true; 
    private Renderer rend;
    private Color originalColor; 
    

    void Start()
    {
        rend = GetComponent<Renderer>();
        if (rend != null && rend.material.HasProperty("_BaseColor"))
        {
            originalColor = rend.material.GetColor("_BaseColor");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (rend != null && rend.material.HasProperty("_BaseColor"))
        {
            rend.material.SetColor("_BaseColor", touch);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (!changeBack)
            return;

        if (rend != null && rend.material.HasProperty("_BaseColor"))
        {
            rend.material.SetColor("_BaseColor", originalColor);
        }
    }

}
