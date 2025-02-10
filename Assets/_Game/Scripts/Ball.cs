using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [Header("Ball Physics")]
    public float mass = 100f;  // Heavy enough to not be too easily pushed
    public float drag = 0.5f;  // Air resistance
    public float angularDrag = 0.5f;  // Rotational resistance
    public float bounciness = 0.6f;  // How bouncy the ball is
    public float friction = 0.6f;  // Surface friction
    
    [Header("Gameplay")]
    public float minImpactForce = 1f;  // Minimum force needed to move the ball
    public float maxVelocity = 30f;  // Maximum speed the ball can reach

    [Header("Sound Effects")]
    [SerializeField] private AudioClip ballImpactSound;    // General impact sound
    [SerializeField] private AudioClip carImpactSound;     // Specific sound for car hits
    [SerializeField] private float minPitch = 0.8f;        // Minimum pitch for soft impacts
    [SerializeField] private float maxPitch = 1.5f;        // Maximum pitch for hard impacts
    [SerializeField] private float minVolume = 0.3f;       // Minimum volume for soft impacts
    [SerializeField] private float maxVolume = 1.0f;       // Maximum volume for hard impacts
    [SerializeField] private float maxImpactForce = 100f;  // Force that would cause max volume/pitch
    [SerializeField] private float carSpeedMultiplier = 2f; // Multiplier for car impact sounds

    private Rigidbody rb;
    private PhysicMaterial physicsMaterial;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        // Setup Rigidbody
        rb = GetComponent<Rigidbody>();
        rb.mass = mass;
        rb.drag = drag;
        rb.angularDrag = angularDrag;
        rb.interpolation = RigidbodyInterpolation.Interpolate; // Smoother movement
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous; // Better collision detection
        
        // Create and setup physics material
        physicsMaterial = new PhysicMaterial("BallPhysicsMaterial");
        physicsMaterial.bounciness = bounciness;
        physicsMaterial.dynamicFriction = friction;
        physicsMaterial.staticFriction = friction;
        physicsMaterial.bounceCombine = PhysicMaterialCombine.Average;
        physicsMaterial.frictionCombine = PhysicMaterialCombine.Average;
        
        // Apply physics material to collider
        GetComponent<Collider>().material = physicsMaterial;

        // Setup AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1f; // Make sound 3D
        audioSource.maxDistance = 20f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.playOnAwake = false;
    }

    void FixedUpdate()
    {
        // Limit maximum velocity
        if (rb.velocity.magnitude > maxVelocity)
        {
            rb.velocity = rb.velocity.normalized * maxVelocity;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        float impactForce = collision.impulse.magnitude;
        
        // Only process significant collisions
        if (impactForce > minImpactForce)
        {
            float normalizedForce;
            float volume;
            float pitch;

            // Special handling for car collisions
            if (collision.gameObject.CompareTag("Car"))
            {
                // Get car's velocity for more dramatic effect
                Rigidbody carRb = collision.gameObject.GetComponent<Rigidbody>();
                float carSpeed = carRb != null ? carRb.velocity.magnitude : 0f;
                
                // Amplify the impact force based on car speed
                float amplifiedForce = impactForce * (1f + (carSpeed / 10f) * carSpeedMultiplier);
                normalizedForce = Mathf.Clamp01(amplifiedForce / maxImpactForce);
                
                // More dramatic volume and pitch for car impacts
                volume = Mathf.Lerp(minVolume, maxVolume, normalizedForce * 1.2f); // Can go 20% louder
                pitch = Mathf.Lerp(minPitch, maxPitch, normalizedForce);

                // Add a slight upward force to make the ball hop on impact
                Vector3 bounceForce = Vector3.up * (impactForce * 0.1f);
                rb.AddForce(bounceForce, ForceMode.Impulse);
            }
            else
            {
                // Normal collision handling
                normalizedForce = Mathf.Clamp01(impactForce / maxImpactForce);
                volume = Mathf.Lerp(minVolume, maxVolume, normalizedForce);
                pitch = Mathf.Lerp(minPitch, maxPitch, normalizedForce);
            }

            // Select appropriate sound
            AudioClip soundToPlay = collision.gameObject.CompareTag("Car") ? carImpactSound : ballImpactSound;

            // Play the sound if we have a clip
            if (soundToPlay != null && audioSource != null)
            {
                audioSource.pitch = pitch;
                audioSource.volume = volume;
                audioSource.PlayOneShot(soundToPlay);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
