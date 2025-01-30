using UnityEngine;

public class PopEffect : MonoBehaviour
{
    private ParticleSystem[] particles;
    
    void Start()
    {
        particles = GetComponentsInChildren<ParticleSystem>();
        float longestDuration = 0f;
        
        // Find the longest duration among all particle systems
        foreach (var ps in particles)
        {
            float duration = ps.main.duration + ps.main.startLifetime.constantMax;
            longestDuration = Mathf.Max(longestDuration, duration);
        }
        
        // Destroy after all particles are done
        Destroy(gameObject, longestDuration);
    }
} 