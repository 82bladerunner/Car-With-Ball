using System;
using UnityEngine;

public class TileMove : MonoBehaviour
{
    [Header("Movement Settings")]
    public Vector3 endPoint = new Vector3(0, 2, 0); // Distance to move relative to start
    public float speed = 2f;
    public float waitTime = 0.5f; // Time to wait at each point
    
    [Header("Movement Type")]
    public bool smoothMovement = true; // Toggle between smooth or linear movement
    
    private Vector3 startPoint;
    private float waitCounter;
    private float moveProgress = 0f;
    private bool movingToEnd = true;

    // Start is called before the first frame update
    void Start()
    {
        startPoint = transform.position;
        waitCounter = waitTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.TryGetComponent(out CarReferences carReferences))
        {
            // carReferences.rigidbody.velocity = Vector3.zero;
            // carReferences.rigidbody.angularVelocity = Vector3.zero;
            
            foreach (var wc in carReferences.wheelColliders)
            {
                wc.motorTorque = 0;
                wc.brakeTorque = 0;
            }
            
            carReferences.holderObject.SetParent(transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.TryGetComponent(out CarReferences carReferences))
        {
            carReferences.holderObject.SetParent(null);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (waitCounter > 0)
        {
            waitCounter -= Time.deltaTime;
            return;
        }

        if (movingToEnd)
        {
            moveProgress += Time.deltaTime * speed;
            if (moveProgress >= 1f)
            {
                moveProgress = 1f;
                movingToEnd = false;
                waitCounter = waitTime;
            }
        }
        else
        {
            moveProgress -= Time.deltaTime * speed;
            if (moveProgress <= 0f)
            {
                moveProgress = 0f;
                movingToEnd = true;
                waitCounter = waitTime;
            }
        }

        if (smoothMovement)
        {
            // Smooth movement using sine interpolation
            float smoothProgress = (Mathf.Sin(moveProgress * Mathf.PI - Mathf.PI/2) + 1f) / 2f;
            transform.position = Vector3.Lerp(startPoint, startPoint + endPoint, smoothProgress);
        }
        else
        {
            // Linear movement
            transform.position = Vector3.Lerp(startPoint, startPoint + endPoint, moveProgress);
        }
    }

    // Optional: Visualize the path in the editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 start = Application.isPlaying ? startPoint : transform.position;
        Vector3 end = start + endPoint;
        Gizmos.DrawLine(start, end);
        Gizmos.DrawWireSphere(start, 0.2f);
        Gizmos.DrawWireSphere(end, 0.2f);
    }
}
