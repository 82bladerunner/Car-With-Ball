using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    [Header("Pop Effect")]
    [SerializeField] private GameObject popEffect;    // Assign a particle system prefab
    [SerializeField] private AudioClip popSound;      // Assign a pop sound

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 180f;  // Degrees per second
    [SerializeField] private Vector3 rotationAxis = Vector3.up;  // Which axis to rotate around
    [SerializeField] private bool reverseRotation = false;  // Direction of rotation

    [Header("Warning Effect")]
    [SerializeField] private float glowIntensity = 1.5f;  // How bright the warning glow is
    [SerializeField] private Color warningColor = Color.red;
    private Material spikeMaterial;
    private Color originalEmissionColor;

    // Start is called before the first frame update
    void Start()
    {
        // Get the material to control emission
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            spikeMaterial = renderer.material;
            if (spikeMaterial != null)
            {
                spikeMaterial.EnableKeyword("_EMISSION");
                originalEmissionColor = spikeMaterial.GetColor("_EmissionColor");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Apply rotation
        float direction = reverseRotation ? -1f : 1f;
        transform.Rotate(rotationAxis, rotationSpeed * direction * Time.deltaTime);

        // Pulse warning glow
        if (spikeMaterial != null)
        {
            float pulse = (Mathf.Sin(Time.time * 2f) + 1f) * 0.5f;  // Value between 0 and 1
            Color glowColor = Color.Lerp(originalEmissionColor, warningColor * glowIntensity, pulse);
            spikeMaterial.SetColor("_EmissionColor", glowColor);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            Debug.Log("Ball hit spike!"); // Debug log

            Vector3 contactPoint = collision.contacts[0].point;
            
            // Create pop effect
            if (popEffect != null)
            {
                GameObject effect = Instantiate(popEffect, contactPoint, Quaternion.identity);
                effect.transform.localScale = Vector3.one * 3f; // Make effect larger
            }

            // Play sound
            if (popSound != null)
            {
                AudioSource.PlayClipAtPoint(popSound, transform.position);
            }

            // Make the ball explode
            StartCoroutine(PopBall(collision.gameObject));

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
