using UnityEngine;

public class PlantSwayEffect : MonoBehaviour
{
    [Header("Sway Settings")]
    [Range(0.1f, 5f)]
    public float swaySpeed = 1f;
    [Range(0f, 30f)]
    public float swayAmount = 5f;
    [Range(0f, 1f)]
    public float noiseInfluence = 0.5f;
    [Range(0f, 2f)]
    public float randomOffset = 1f;

    private float timeOffset;
    private Vector3 originalRotation;
    private float noiseOffset;

    void Start()
    {
        // Store the original rotation
        originalRotation = transform.localEulerAngles;
        
        // Generate random offsets for more natural movement
        timeOffset = Random.Range(0f, 1000f);
        noiseOffset = Random.Range(0f, 1000f);
    }

    void Update()
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

        // Apply final rotation
        transform.localEulerAngles = new Vector3(
            originalRotation.x + totalSwayX + randomX,
            originalRotation.y,
            originalRotation.z + totalSwayZ + randomZ
        );
    }
} 