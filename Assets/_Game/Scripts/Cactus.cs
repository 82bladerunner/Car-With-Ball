using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cactus : MonoBehaviour
{
    [SerializeField] private GameObject popEffect; // Assign a particle system prefab
    [SerializeField] private AudioClip popSound;   // Assign a pop sound
    [SerializeField] private float explosionForce = 1000f;
    [SerializeField] private float explosionRadius = 5f;

    [Header("Warning Flash")]
    [SerializeField] private float flashSpeed = 2f;
    [SerializeField] private Color warningColor = Color.red;
    private GameObject warningGlow;
    private float flashTimer;
    private Material originalMaterial; // Store reference to original material

    // Fixed wind sway settings - not adjustable in editor
    private const float swaySpeed = 0.8f;      // Slower movement
    private const float swayAmount = 8f;       // Subtle sway
    private const float noiseInfluence = 0.3f; // Light noise
    private const float randomOffset = 0.5f;   // Light random movement

    private float timeOffset;
    private float noiseOffset;
    private Vector3 originalRotation;

    // Start is called before the first frame update
    void Start()
    {
        // Store the original material
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer != null && renderer.material != null)
        {
            originalMaterial = renderer.material;
        }
        
        CreateWarningGlow();

        // Initialize wind sway
        originalRotation = transform.localEulerAngles;
        timeOffset = Random.Range(0f, 1000f);
        noiseOffset = Random.Range(0f, 1000f);
    }

    void CreateWarningGlow()
    {
        warningGlow = new GameObject("WarningGlow");
        warningGlow.transform.parent = transform;
        warningGlow.transform.localPosition = Vector3.zero;
        warningGlow.transform.localRotation = Quaternion.identity;
        
        MeshFilter originalMesh = GetComponent<MeshFilter>();
        if (originalMesh != null && originalMesh.mesh != null)
        {
            MeshFilter glowMesh = warningGlow.AddComponent<MeshFilter>();
            glowMesh.mesh = originalMesh.mesh;

            MeshRenderer glowRenderer = warningGlow.AddComponent<MeshRenderer>();
            Material glowMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            glowMaterial.SetFloat("_Surface", 1); // Transparent
            glowMaterial.EnableKeyword("_EMISSION");
            
            // Copy original material's base color if available
            if (originalMaterial != null)
            {
                Color baseColor = originalMaterial.GetColor("_BaseColor");
                baseColor.a = 0.2f; // Make it transparent
                glowMaterial.SetColor("_BaseColor", baseColor);
            }
            else
            {
                glowMaterial.SetColor("_BaseColor", new Color(1, 1, 1, 0.2f));
            }
            
            glowRenderer.material = glowMaterial;
            warningGlow.transform.localScale = Vector3.one * 1.05f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Update warning glow
        if (warningGlow != null)
        {
            flashTimer += Time.deltaTime * flashSpeed;
            float intensity = (Mathf.Sin(flashTimer * Mathf.PI) + 1f) * 0.5f;
            
            MeshRenderer glowRenderer = warningGlow.GetComponent<MeshRenderer>();
            if (glowRenderer != null)
            {
                // Lerp between original color and warning color
                Color originalColor = originalMaterial != null ? 
                    originalMaterial.GetColor("_BaseColor") : Color.white;
                Color currentColor = Color.Lerp(originalColor, warningColor, intensity);
                currentColor.a = 0.2f + (intensity * 0.3f);
                
                glowRenderer.material.SetColor("_EmissionColor", currentColor);
                glowRenderer.material.SetColor("_BaseColor", currentColor);
            }
        }

        // Calculate and apply wind sway
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            Debug.Log("Ball hit cactus!"); // Debug log to verify collision

            Vector3 contactPoint = collision.contacts[0].point;
            
            // Create dramatic pop effect
            if (popEffect != null)
            {
                GameObject effect = Instantiate(popEffect, contactPoint, Quaternion.identity);
                effect.transform.localScale = Vector3.one * 3f; // Make effect larger
            }

            // Make the ball explode
            StartCoroutine(PopBall(collision.gameObject));

            // Play sound
            if (popSound != null)
            {
                AudioSource.PlayClipAtPoint(popSound, transform.position);
            }

            // Game over
            GameManager.Instance.GameOver();
        }
    }

    private System.Collections.IEnumerator PopBall(GameObject ball)
    {
        // Disable collider
        Collider ballCollider = ball.GetComponent<Collider>();
        if (ballCollider != null) ballCollider.enabled = false;

        // Scale up quickly
        float duration = 0.15f;
        float elapsed = 0f;
        Vector3 startScale = ball.transform.localScale;
        Vector3 targetScale = startScale * 1.5f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            ball.transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        // Hide the ball
        ball.SetActive(false);
    }
}
