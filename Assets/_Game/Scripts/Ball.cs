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

    private Rigidbody rb;
    private PhysicMaterial physicsMaterial;

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
        // Check if collision is with the car
        if (collision.gameObject.CompareTag("Car"))
        {
            float impactForce = collision.impulse.magnitude;
            
            // Only react to significant impacts
            if (impactForce > minImpactForce)
            {
                // Add a slight upward force to make the ball hop on impact
                Vector3 bounceForce = Vector3.up * (impactForce * 0.1f);
                rb.AddForce(bounceForce, ForceMode.Impulse);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
