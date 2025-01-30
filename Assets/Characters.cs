using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Characters : MonoBehaviour
{
    private Transform carTransform;
    private Vector3 originalScale;
    private float breatheTimer;

    // Breathing animation settings
    private const float breatheSpeed = 1.5f;         // Speed of breathing
    private const float breatheAmount = 0.02f;       // How much the character scales during breathing
    private const float swaySpeed = 0.8f;           // Speed of subtle swaying
    private const float swayAmount = 2f;            // Amount of rotation sway

    // Head turning settings
    private const float maxLookAngle = 8f;          // Maximum degrees to look left/right
    private const float minLookTime = 4f;           // Minimum time before changing look direction
    private const float maxLookTime = 10f;          // Maximum time before changing look direction
    private const float turnSpeed = 0.3f;           // How fast to turn the head
    private const float carTrackingSpeed = 0.5f;    // How fast to turn towards car
    
    private Vector3 originalRotation;
    private float timeOffset;
    private float currentLookAngle;
    private float targetLookAngle;
    private float nextLookTime;
    private float currentYRotation;

    // Start is called before the first frame update
    void Start()
    {
        // Find the car (assuming it has PrometeoCarController component)
        var carController = FindObjectOfType<PrometeoCarController>();
        if (carController != null)
        {
            carTransform = carController.transform;
        }

        // Store original scale and rotation for breathing animation
        originalScale = transform.localScale;
        originalRotation = transform.eulerAngles;
        currentYRotation = originalRotation.y;
        
        // Random offset for more natural movement
        timeOffset = Random.Range(0f, 1000f);
        breatheTimer = Random.Range(0f, Mathf.PI * 2); // Random start phase

        // Initialize looking behavior
        nextLookTime = Time.time + Random.Range(minLookTime, maxLookTime);
        currentLookAngle = 0f;
        targetLookAngle = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        // Update head turning
        if (Time.time >= nextLookTime)
        {
            // Set new random look angle
            targetLookAngle = Random.Range(-maxLookAngle, maxLookAngle);
            nextLookTime = Time.time + Random.Range(minLookTime, maxLookTime);
        }

        // Smoothly interpolate to target look angle
        currentLookAngle = Mathf.Lerp(currentLookAngle, targetLookAngle, Time.deltaTime * turnSpeed);

        // Face the car (base rotation)
        if (carTransform != null)
        {
            Vector3 directionToCar = carTransform.position - transform.position;
            directionToCar.y = 0; // Keep character upright, only rotate on Y axis
            
            if (directionToCar != Vector3.zero)
            {
                float targetYRotation = Quaternion.LookRotation(directionToCar).eulerAngles.y;
                currentYRotation = Mathf.LerpAngle(currentYRotation, targetYRotation, Time.deltaTime * carTrackingSpeed);
            }
        }

        // Breathing effect
        breatheTimer += Time.deltaTime * breatheSpeed;
        float breatheScale = 1f + Mathf.Sin(breatheTimer) * breatheAmount;
        
        // Apply breathing scale
        transform.localScale = new Vector3(
            originalScale.x * breatheScale,
            originalScale.y * breatheScale,
            originalScale.z * breatheScale
        );

        // Add subtle swaying
        float swayX = Mathf.Sin((Time.time + timeOffset) * swaySpeed) * swayAmount;
        float swayZ = Mathf.Cos((Time.time + timeOffset) * swaySpeed * 0.6f) * swayAmount * 0.5f;

        // Apply all rotations
        transform.rotation = Quaternion.Euler(
            originalRotation.x + swayX,
            currentYRotation + currentLookAngle,
            originalRotation.z + swayZ
        );
    }
}
