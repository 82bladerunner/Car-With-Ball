using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance { get; private set; }
    
    [SerializeField] private Image fadePanel;
    [SerializeField] private float fadeDuration = 1f;
    private bool isTransitioning = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // Keep the fade panel canvas
            if (fadePanel != null)
            {
                DontDestroyOnLoad(fadePanel.transform.root.gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }

        // Make sure panel starts transparent
        if (fadePanel != null)
        {
            fadePanel.color = new Color(0, 0, 0, 0);
        }
    }

    public void RestartScene()
    {
        if (!isTransitioning)
        {
            StartCoroutine(FadeAndRestart());
        }
    }

    private IEnumerator FadeAndRestart()
    {
        if (fadePanel == null) yield break;
        
        isTransitioning = true;

        // Fade to black
        float elapsedTime = 0;
        while (elapsedTime < fadeDuration)
        {
            if (fadePanel == null) yield break;
            
            elapsedTime += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            fadePanel.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        // Reset timescale and reload scene
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        // Wait one frame for scene to load
        yield return null;

        // Fade back in
        elapsedTime = 0;
        while (elapsedTime < fadeDuration)
        {
            if (fadePanel == null) yield break;
            
            elapsedTime += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            fadePanel.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        isTransitioning = false;
    }

    public void LoadScene(string sceneName)
    {
        if (!isTransitioning)
        {
            StartCoroutine(FadeAndLoadScene(sceneName));
        }
    }

    private IEnumerator FadeAndLoadScene(string sceneName)
    {
        if (fadePanel == null) yield break;
        
        isTransitioning = true;

        // Fade to black
        float elapsedTime = 0;
        while (elapsedTime < fadeDuration)
        {
            if (fadePanel == null) yield break;
            
            elapsedTime += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            fadePanel.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        // Load new scene
        SceneManager.LoadScene(sceneName);

        // Wait one frame for scene to load
        yield return null;

        // Fade back in
        elapsedTime = 0;
        while (elapsedTime < fadeDuration)
        {
            if (fadePanel == null) yield break;
            
            elapsedTime += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            fadePanel.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        isTransitioning = false;
    }
} 