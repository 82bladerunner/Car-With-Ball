using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // For scene reloading

public class Star : MonoBehaviour
{
    [Header("Collection Effects")]
    [SerializeField] private AudioClip collectSound;
    [SerializeField] private ParticleSystem collectEffect;
    [SerializeField] private float collectSoundVolume = 1f;

    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        // Setup audio source
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1f; // 3D sound
        audioSource.playOnAwake = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            // Play collection sound
            if (collectSound != null)
            {
                AudioSource.PlayClipAtPoint(collectSound, transform.position, collectSoundVolume);
            }

            // Play particle effect
            if (collectEffect != null)
            {
                ParticleSystem effect = Instantiate(collectEffect, transform.position, Quaternion.identity);
                effect.Play();
                Destroy(effect.gameObject, effect.main.duration + 0.5f);
            }

            // Complete level
            GameManager.Instance.CompleteLevel();
            
            // Hide the star
            gameObject.SetActive(false);
        }
    }

    private System.Collections.IEnumerator GameOver()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
