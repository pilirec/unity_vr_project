using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class ScalableGrabbable_InputSystem : MonoBehaviour
{
    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor interactor;

    [Header("Input Action References")]
    public InputActionReference leftTriggerAction;
    public InputActionReference rightAPressedAction;
    public InputActionReference rightBPressedAction;

    public float scaleSpeed = 0.1f;
    public float minScale = 0.1f;
    public float maxScale = 3f;

    private bool isGrabbing = false;

    void OnEnable()
    {
        leftTriggerAction.action.performed += OnLeftTriggerPerformed;
        leftTriggerAction.action.canceled += OnLeftTriggerCanceled;
        rightAPressedAction.action.performed += OnRightAPressed;
        rightBPressedAction.action.performed += OnRightBPressed;
    }

    void OnDisable()
    {
        leftTriggerAction.action.performed -= OnLeftTriggerPerformed;
        leftTriggerAction.action.canceled -= OnLeftTriggerCanceled;
        rightAPressedAction.action.performed -= OnRightAPressed;
        rightBPressedAction.action.performed -= OnRightBPressed;
    }

    void OnLeftTriggerPerformed(InputAction.CallbackContext context)
    {
        isGrabbing = true;
        Debug.Log("Left trigger pressed, grabbing started.");
    }

    void OnLeftTriggerCanceled(InputAction.CallbackContext context)
    {
        isGrabbing = false;
        Debug.Log("Left trigger released, grabbing ended.");
    }

    void OnRightAPressed(InputAction.CallbackContext context)
    {
        if (isGrabbing)
            ScaleObject(1);
    }

    void OnRightBPressed(InputAction.CallbackContext context)
    {
        if (isGrabbing)
            ScaleObject(-1);
    }

    void ScaleObject(int direction)
    {
        Vector3 newScale = transform.localScale + Vector3.one * direction * scaleSpeed * Time.deltaTime;
        newScale = Vector3.Max(Vector3.one * minScale, Vector3.Min(Vector3.one * maxScale, newScale));
        transform.localScale = newScale;
        Debug.Log($"Scaled object to {transform.localScale}");
    }

    public void OnSelectEntered(SelectEnterEventArgs args)
    {
        interactor = args.interactorObject as UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor;
    }

    public void OnSelectExited(SelectExitEventArgs args)
    {
        interactor = null;
    }
}
