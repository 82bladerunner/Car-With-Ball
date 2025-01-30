using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class GhostCar : MonoBehaviour
{
    private List<CarSnapshot> replayData;
    private float replayStartTime;
    private int currentIndex = 0;
    private Material ghostMaterial;
    private float fadeOutDuration = 1f;
    private bool isFading = false;

    public void Initialize(List<CarSnapshot> data)
    {
        replayData = new List<CarSnapshot>(data);
        replayStartTime = Time.time;
        currentIndex = 0;

        // Create transparent ghost material
        ghostMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        ghostMaterial.SetFloat("_Surface", 1); // 1 = Transparent
        ghostMaterial.SetFloat("_Blend", 0);   // 0 = Alpha blend
        ghostMaterial.SetShaderPassEnabled("ShadowCaster", false); // Don't cast shadows
        
        // Set transparent color (keep original color but make it transparent)
        Color ghostColor = new Color(0.5f, 0.5f, 1f, 0.3f); // Light blue transparent
        ghostMaterial.SetColor("_BaseColor", ghostColor);
        ghostMaterial.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        ghostMaterial.renderQueue = 3000; // Transparent queue

        // Apply to all renderers
        foreach (MeshRenderer renderer in GetComponentsInChildren<MeshRenderer>())
        {
            renderer.material = ghostMaterial;
        }
    }

    void Update()
    {
        if (replayData == null || (currentIndex >= replayData.Count && !isFading)) 
        {
            StartFadeOut();
            return;
        }

        if (!isFading)
        {
            float currentTime = Time.time - replayStartTime;

            while (currentIndex < replayData.Count && currentTime >= replayData[currentIndex].timestamp)
            {
                transform.position = replayData[currentIndex].position;
                transform.rotation = replayData[currentIndex].rotation;
                currentIndex++;
            }
        }
    }

    private void StartFadeOut()
    {
        if (!isFading)
        {
            isFading = true;
            StartCoroutine(FadeOut());
        }
    }

    private System.Collections.IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        Color startColor = ghostMaterial.color;

        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startColor.a, 0f, elapsedTime / fadeOutDuration);
            ghostMaterial.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        Destroy(gameObject);
    }
} 