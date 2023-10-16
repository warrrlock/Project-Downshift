using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CameraTarget
{
    public Transform target; // Reference to the target's transform
    public float distance = 5f; // Distance from the target
    public float height = 2f; // Height above the target
    public float smoothSpeed = 0.125f; // Smoothing speed for position
    public bool lookAtTarget; // Toggle for smoothing look at
}

public class CarCamera : MonoBehaviour
{
    public CameraTarget[] targets; // Array of camera targets
    private int currentTargetIndex = 0; // Index of the current target

    private Vector3 velocity = Vector3.zero;


    private void Update()
    {
        // Check for keypress to switch targets
        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.JoystickButton5))
        {
            SwitchTarget();
        }
        
    }

    private void FixedUpdate()
    {
        CamFollow();

    }

    private void CamFollow()
    {
        // Make sure there are targets in the array
        if (targets == null || targets.Length == 0)
        {
            Debug.LogWarning("No targets assigned for the camera!");
            return;
        }

        // Get the current target
        CameraTarget currentTarget = targets[currentTargetIndex];

        // Calculate the desired position for the camera
        Vector3 targetPosition = currentTarget.target.position;
        Vector3 desiredPosition = targetPosition - currentTarget.target.forward * currentTarget.distance + Vector3.up * currentTarget.height;

        // Smoothly move the camera towards the desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, currentTarget.smoothSpeed);
        transform.position = smoothedPosition;

        if (currentTarget.lookAtTarget == true)
        {
            // Make the camera look at the current target
            transform.LookAt(targetPosition + Vector3.up * currentTarget.height);
        }
        else
        {         //transform.position = currentTarget.target.position;
            transform.rotation = currentTarget.target.rotation;
        }

    }


    private void SwitchTarget()
    {
        // Increment the target index and wrap around if needed
        currentTargetIndex = (currentTargetIndex + 1) % targets.Length;
    }
}
