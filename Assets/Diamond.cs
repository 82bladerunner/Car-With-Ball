using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Diamond : MonoBehaviour
{
    [Header("Collection Effect")]
    [SerializeField] private float rotationSpeed = 90f;    // Degrees per second
    [SerializeField] private float floatAmplitude = 0.2f;  // How high it floats
    [SerializeField] private float floatSpeed = 2f;        // Float cycle speed
    [SerializeField] private ParticleSystem collectEffect;  // Optional particle effect
    [SerializeField] private AudioClip collectSound;       // Optional collection sound

    private Vector3 startPosition;
    private float floatOffset;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
        floatOffset = Random.Range(0f, 2f * Mathf.PI); // Random start phase

        // Make sure we have a trigger collider
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        if (boxCollider != null && !boxCollider.isTrigger)
        {
            boxCollider.isTrigger = true;
            Debug.Log("Set BoxCollider to trigger mode");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Rotate the diamond
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        // Make it float up and down
        float newY = startPosition.y + Mathf.Sin((Time.time + floatOffset) * floatSpeed) * floatAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Trigger entered by: {other.gameObject.name} with tag: {other.tag}"); // Debug log

        if (other.CompareTag("Car"))
        {
            Debug.Log("Car tag detected - collecting diamond"); // Debug log
            CollectDiamond();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Collision entered by: {collision.gameObject.name} with tag: {collision.gameObject.tag}"); // Debug log

        if (collision.gameObject.CompareTag("Car"))
        {
            Debug.Log("Car tag detected in collision - collecting diamond"); // Debug log
            CollectDiamond();
        }
    }

    private void CollectDiamond()
    {
        // Increment diamond count
        GameManager.Instance.CollectDiamond();

        // Play collection effect if assigned
        if (collectEffect != null)
        {
            ParticleSystem effect = Instantiate(collectEffect, transform.position, Quaternion.identity);
            effect.Play();
        }

        // Play sound if assigned
        if (collectSound != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position);
        }

        // Destroy the diamond
        Destroy(gameObject);
    }
}
