using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class WorldScaler : MonoBehaviour
{
    public XRBaseController leftController;
    public XRBaseController rightController;
    public Transform worldToScale;
    
    public InputActionReference scaleUpAction;   // B button
    public InputActionReference scaleDownAction; // A button

    public float scaleSpeed = 0.5f; // how fast the world scales

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabbedObject;

    void OnEnable()
    {
        scaleUpAction.action.Enable();
        scaleDownAction.action.Enable();
    }

    void OnDisable()
    {
        scaleUpAction.action.Disable();
        scaleDownAction.action.Disable();
    }

    void Update()
    {
        if (grabbedObject != null)
        {
            float scaleDelta = 0;

            if (scaleUpAction.action.IsPressed())
                scaleDelta += scaleSpeed * Time.deltaTime;
            if (scaleDownAction.action.IsPressed())
                scaleDelta -= scaleSpeed * Time.deltaTime;

            if (Mathf.Abs(scaleDelta) > 0)
            {
                Vector3 currentScale = worldToScale.localScale;
                Vector3 newScale = currentScale + Vector3.one * scaleDelta;
                newScale = Vector3.Max(newScale, Vector3.one * 0.1f); // avoid zero scale
                worldToScale.localScale = newScale;
            }
        }
    }

    public void SetGrabbedObject(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable interactable)
    {
        grabbedObject = interactable;
    }

    public void ClearGrabbedObject(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable interactable)
    {
        if (grabbedObject == interactable)
            grabbedObject = null;
    }
}
