using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    [Header("Star Particles")]
    [SerializeField] private int starCount = 1000;
    [SerializeField] private float starSpawnRadius = 50f;
    [SerializeField] private float starMinSize = 0.1f;
    [SerializeField] private float starMaxSize = 0.3f;
    
    [Header("Dust Particles")]
    [SerializeField] private int dustCount = 500;
    [SerializeField] private float dustSpawnRadius = 30f;
    
    void Start()
    {
        SetupStars();
        SetupDust();
    }
    
    void SetupStars()
    {
        GameObject stars = new GameObject("Stars");
        stars.transform.parent = transform;
        
        ParticleSystem.MainModule mainModule;
        ParticleSystem starPS = stars.AddComponent<ParticleSystem>();
        mainModule = starPS.main;
        mainModule.loop = true;
        mainModule.startLifetime = Mathf.Infinity;
        mainModule.startSpeed = 0;
        mainModule.startSize = Random.Range(starMinSize, starMaxSize);
        mainModule.maxParticles = starCount;
        
        ParticleSystem.EmissionModule emission = starPS.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[]{ new ParticleSystem.Burst(0f, starCount) });
        
        ParticleSystem.ShapeModule shape = starPS.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = starSpawnRadius;
        shape.randomDirectionAmount = 1f;
    }
    
    void SetupDust()
    {
        GameObject dust = new GameObject("Dust");
        dust.transform.parent = transform;
        
        ParticleSystem dustPS = dust.AddComponent<ParticleSystem>();
        var main = dustPS.main;
        main.loop = true;
        main.startLifetime = 5f;
        main.startSpeed = 0.5f;
        main.startSize = 0.1f;
        main.maxParticles = dustCount;
        
        var emission = dustPS.emission;
        emission.rateOverTime = dustCount / 5f;
        
        var shape = dustPS.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = dustSpawnRadius;
        
        var col = dustPS.colorOverLifetime;
        col.enabled = true;
        Gradient grad = new Gradient();
        grad.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.white, 0.0f), new GradientColorKey(Color.white, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(0, 0.0f), new GradientAlphaKey(0.5f, 0.2f), new GradientAlphaKey(0, 1.0f) }
        );
        col.color = grad;
    }
} 