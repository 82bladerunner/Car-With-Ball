using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    public float levelStartTime { get; private set; }
    public float levelCompletionTime { get; private set; }
    public int diamondsCollected { get; private set; }

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI diamondCountText;

    private bool isGameOver = false;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartLevel();
    }
    
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void Update()
    {
        // Update timer only if game is not over
        if (timerText != null && !isGameOver)
        {
            float currentTime = Time.time - levelStartTime;
            timerText.text = $"Time: {currentTime:F2}";
        }
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void StartLevel()
    {
        Time.timeScale = 1f;
        levelStartTime = Time.time;
        isGameOver = false;
        diamondsCollected = 0;
        UpdateDiamondUI();
    }

    public void CollectDiamond()
    {
        diamondsCollected++;
        UpdateDiamondUI();
    }

    private void UpdateDiamondUI()
    {
        if (diamondCountText != null)
        {
            diamondCountText.text = $"Diamonds: {diamondsCollected}";
        }
    }

    public void CompleteLevel()
    {
        if (isGameOver) return;
        
        isGameOver = true;
        levelCompletionTime = Time.time - levelStartTime;
        Debug.Log($"Level completed in: {levelCompletionTime:F2} seconds");
        
        // Slow down time
        Time.timeScale = 0.3f;
    }

    public void GameOver()
    {
        if (isGameOver) return;
        
        isGameOver = true;
        levelCompletionTime = Time.time - levelStartTime;
        
        // Slow down time
        Time.timeScale = 0.3f;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reset time scale
        Time.timeScale = 1f;
      
        StartLevel();
    }
} 