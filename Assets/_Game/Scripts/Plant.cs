using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    [Header("Grass Movement")]
    [SerializeField] private float bendRadius = 2f;         // How close objects need to be to affect the plant
    [SerializeField] private float bendStrength = 45f;      // Maximum bend angle in degrees
    [SerializeField] private float recoverySpeed = 2f;      // How fast the plant returns to original position
    [SerializeField] private float heightInfluence = 1f;    // How much height difference affects the bend

    // Fixed wind sway settings - not adjustable in editor
    private const float swaySpeed = 1.2f;        // Moderate speed
    private const float swayAmount = 12f;        // Moderate sway amount
    private const float noiseInfluence = 0.4f;   // Subtle noise
    private const float randomOffset = 0.8f;     // Subtle random movement

    private Quaternion originalRotation;
    private Quaternion targetRotation;
    private Vector3 currentVelocity;
    private float timeOffset;
    private float noiseOffset;

    // Start is called before the first frame update
    void Start()
    {
        originalRotation = transform.rotation;
        targetRotation = originalRotation;
        
        // Generate random offsets for wind movement
        timeOffset = Random.Range(0f, 1000f);
        noiseOffset = Random.Range(0f, 1000f);
    }

    // Update is called once per frame
    void Update()
    {
        // Calculate wind sway
        Quaternion windSway = CalculateWindSway();
        
        // Check for nearby objects
        Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, bendRadius);
        Vector3 totalBendDirection = Vector3.zero;
        bool shouldBend = false;

        foreach (Collider col in nearbyObjects)
        {
            // Check for ball tag or car component
            if (col.CompareTag("Ball") || col.GetComponentInParent<PrometeoCarController>() != null)
            {
                Vector3 directionToObject = transform.position - col.transform.position;
                directionToObject.y = 0; // Keep bending on horizontal plane

                // Calculate bend strength based on distance
                float distance = directionToObject.magnitude;
                float bendFactor = 1 - (distance / bendRadius);
                bendFactor = Mathf.Clamp01(bendFactor);

                // Add to total bend direction
                totalBendDirection += directionToObject.normalized * bendFactor;
                shouldBend = true;

                // Add velocity influence if available
                Rigidbody rb = col.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    totalBendDirection += rb.velocity.normalized * rb.velocity.magnitude * 0.1f;
                }
            }
        }

        // Calculate interaction-based rotation
        if (shouldBend && totalBendDirection != Vector3.zero)
        {
            // Create rotation to bend away from objects
            Quaternion bendRotation = Quaternion.FromToRotation(Vector3.up, Vector3.up + totalBendDirection.normalized * bendStrength);
            targetRotation = bendRotation * originalRotation;
        }
        else
        {
            // Return to original rotation
            targetRotation = originalRotation;
        }

        // Combine wind sway with interaction-based rotation
        Quaternion finalRotation = Quaternion.Lerp(transform.rotation, targetRotation * windSway, recoverySpeed * Time.deltaTime);
        transform.rotation = finalRotation;
    }

    private Quaternion CalculateWindSway()
    {
        // Calculate base sway using sine wave
        float baseSwayX = Mathf.Sin((Time.time + timeOffset) * swaySpeed) * swayAmount;
        float baseSwayZ = Mathf.Cos((Time.time + timeOffset) * swaySpeed * 0.7f) * swayAmount;

        // Add Perlin noise for unpredictability
        float noiseX = (Mathf.PerlinNoise(Time.time * swaySpeed * 0.5f, noiseOffset) - 0.5f) * noiseInfluence * swayAmount;
        float noiseZ = (Mathf.PerlinNoise(noiseOffset, Time.time * swaySpeed * 0.5f) - 0.5f) * noiseInfluence * swayAmount;

        // Combine base sway with noise
        float totalSwayX = baseSwayX + noiseX;
        float totalSwayZ = baseSwayZ + noiseZ;

        // Add random subtle variations
        float randomX = Mathf.Sin(Time.time * Random.Range(0.1f, 0.3f)) * randomOffset;
        float randomZ = Mathf.Cos(Time.time * Random.Range(0.1f, 0.3f)) * randomOffset;

        // Create and return the wind sway rotation
        return Quaternion.Euler(totalSwayX + randomX, 0, totalSwayZ + randomZ);
    }

    // Optional: Visualize the bend radius in the editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.2f);
        Gizmos.DrawWireSphere(transform.position, bendRadius);
    }
}
