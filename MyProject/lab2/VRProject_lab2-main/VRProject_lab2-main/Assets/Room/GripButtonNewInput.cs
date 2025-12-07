using UnityEngine;
using UnityEngine.InputSystem;

public class GripButtonNewInput : MonoBehaviour
{
    public InputActionProperty gripAction;

    void Update()
    {
        if (gripAction.action.ReadValue<float>() > 0.5f)
        {
            Debug.Log("Grip is pressed!");
        }
    }
}
