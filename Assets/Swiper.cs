using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swiper : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotationSpeed = 100f;  // Degrees per second
    public Vector3 rotationAxis = Vector3.up;  // Which axis to rotate around
    public bool clockwise = true;  // Direction of rotation
    
    [Header("Impact Settings")]
    public float hitForce = 1000f;  // Force applied to ball on hit
    public float upwardForce = 100f; // Upward force to prevent ball from getting stuck

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        // Calculate rotation direction
        float direction = clockwise ? -1f : 1f;
        
        // Apply rotation
        transform.Rotate(rotationAxis, rotationSpeed * direction * Time.fixedDeltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if we hit the ball
        if (collision.gameObject.CompareTag("Ball"))
        {
            Rigidbody ballRb = collision.gameObject.GetComponent<Rigidbody>();
            if (ballRb != null)
            {
                // Calculate hit direction based on swiper's current rotation and movement
                Vector3 hitDirection = collision.contacts[0].point - transform.position;
                hitDirection.y = 0; // Keep force horizontal
                hitDirection = hitDirection.normalized;

                // Apply forces
                ballRb.AddForce(hitDirection * hitForce + Vector3.up * upwardForce, ForceMode.Impulse);
            }
        }
    }
}
