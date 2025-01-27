using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationScript : MonoBehaviour
{
    [Header("Rotation Settings")]
    public Vector3 rotationSpeed = new Vector3(0, 90, 0); // Degrees per second
    public bool enableRotation = true;

    [Header("Movement Settings")]
    public bool enableMovement = true;
    public Vector3 movementDirection = Vector3.up; // Direction of oscillation
    public float movementDistance = 1f;           // Distance to move
    public float movementSpeed = 1f;              // Speed multiplier
    public AnimationCurve movementCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // Smoothing curve

    private Vector3 startPosition;
    private float movementTime;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
        
        // Initialize default animation curve if none is set
        if (movementCurve.length == 0)
        {
            movementCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Handle rotation
        if (enableRotation)
        {
            transform.Rotate(rotationSpeed * Time.deltaTime);
        }

        // Handle movement
        if (enableMovement)
        {
            // Update movement time
            movementTime += Time.deltaTime * movementSpeed;
            
            // Calculate smooth movement using sin wave and animation curve
            float factor = Mathf.Sin(movementTime);
            float smoothFactor = movementCurve.Evaluate((factor + 1f) * 0.5f);
            
            // Apply movement
            Vector3 offset = movementDirection.normalized * movementDistance * smoothFactor;
            transform.position = startPosition + offset;
        }
    }

    // Optional: Visualize movement path in editor
    void OnDrawGizmosSelected()
    {
        if (enableMovement)
        {
            Gizmos.color = Color.yellow;
            Vector3 start = Application.isPlaying ? startPosition : transform.position;
            Vector3 end = start + movementDirection.normalized * movementDistance;
            Gizmos.DrawLine(start, end);
            Gizmos.DrawWireSphere(start, 0.1f);
            Gizmos.DrawWireSphere(end, 0.1f);
        }
    }
}
