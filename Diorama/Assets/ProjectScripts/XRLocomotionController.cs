using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;

public class XRLocomotionController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float movementSpeed = 2.0f;
    public Transform headTransform; // Reference to the camera/head

    [Header("Input Actions")]
    public InputActionReference moveInputAction;

    private CharacterController characterController;
    private XROrigin xrOrigin;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        xrOrigin = GetComponent<XROrigin>();

        // If no head transform is assigned, try to find the camera
        if (headTransform == null && xrOrigin != null)
        {
            headTransform = xrOrigin.Camera.transform;
        }
    }

    private void OnEnable()
    {
        if (moveInputAction != null)
            moveInputAction.action.Enable();
    }

    private void OnDisable()
    {
        if (moveInputAction != null)
            moveInputAction.action.Disable();
    }

    private void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (moveInputAction == null || headTransform == null)
            return;

        // Get movement input from the controller
        Vector2 input = moveInputAction.action.ReadValue<Vector2>();

        // Calculate movement direction based on head orientation (ignoring vertical direction)
        Vector3 headForward = headTransform.forward;
        headForward.y = 0;
        headForward.Normalize();

        Vector3 headRight = headTransform.right;
        headRight.y = 0;
        headRight.Normalize();

        // Create movement vector
        Vector3 movement = (headForward * input.y + headRight * input.x) * movementSpeed * Time.deltaTime;

        // Apply movement using character controller
        if (characterController != null)
        {
            characterController.Move(movement);
        }
        else
        {
            // Fallback if no character controller
            transform.position += movement;
        }
    }
}