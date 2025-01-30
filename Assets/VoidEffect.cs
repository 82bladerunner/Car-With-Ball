using UnityEngine;

public class VoidEffect : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float pulseSpeed = 1f;
    [SerializeField] private Color voidColor = new Color(0.1f, 0, 0.2f);
    
    private Material material;

    void Start()
    {
        // Create a large plane or cube below the level
        material = GetComponent<Renderer>().material;
        material.EnableKeyword("_EMISSION");
        material.SetColor("_EmissionColor", voidColor);
    }

    void Update()
    {
        // Rotate the void
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        
        // Pulse the emission
        float emission = 1 + Mathf.Sin(Time.time * pulseSpeed) * 0.5f;
        material.SetColor("_EmissionColor", voidColor * emission);
    }
} 