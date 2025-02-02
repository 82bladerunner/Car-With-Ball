using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    public float levelStartTime { get; private set; }
    public float levelCompletionTime { get; private set; }
    public int diamondsCollected { get; private set; }

    [Header("UI Elements")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private TextMeshProUGUI winText;
    [SerializeField] private TextMeshProUGUI loseText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI diamondCountText;
    [SerializeField] private SceneTransition sceneTransition;

    private bool isGameOver = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Also preserve the UI canvas and transition canvas
            if (winPanel != null && losePanel != null)
            {
                DontDestroyOnLoad(winPanel.transform.root.gameObject);
            }
            if (sceneTransition != null)
            {
                DontDestroyOnLoad(sceneTransition.gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }

        // Hide panels at start
        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);
    }

    private void Start()
    {
        StartLevel();
    }

    public void StartLevel()
    {
        Time.timeScale = 1f;
        levelStartTime = Time.time;
        isGameOver = false;
        diamondsCollected = 0;
        UpdateDiamondUI();
        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);
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
        
        // Show win panel with diamonds collected
        winPanel.SetActive(true);
        winText.text = $"Level Complete!\nTime: {levelCompletionTime:F2}s\nDiamonds: {diamondsCollected}";
    }

    public void GameOver()
    {
        if (isGameOver) return;
        
        isGameOver = true;
        levelCompletionTime = Time.time - levelStartTime;
        
        // Slow down time
        Time.timeScale = 0.3f;
        
        if (losePanel != null)
        {
            // Show lose panel with diamonds collected
            losePanel.SetActive(true);
            if (loseText != null)
            {
                loseText.text = $"You Lost!\nTime: {levelCompletionTime:F2}s\nDiamonds: {diamondsCollected}\nPress Enter to Restart";
            }
        }
    }

    private void Update()
    {
        // Update timer only if game is not over
        if (timerText != null && !isGameOver)
        {
            float currentTime = Time.time - levelStartTime;
            timerText.text = $"Time: {currentTime:F2}";
        }

        // Check if UI elements exist before accessing them
        if (losePanel != null && losePanel.activeSelf && Input.GetKeyDown(KeyCode.Return))
        {
            if (sceneTransition != null)
            {
                sceneTransition.RestartScene();
            }
            else
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reset time scale
        Time.timeScale = 1f;
        
        // Find and reassign UI references when scene reloads
        GameObject winPanelObj = GameObject.Find("WinPanel");
        GameObject losePanelObj = GameObject.Find("LosePanel");
        
        if (winPanelObj != null && losePanelObj != null)
        {
            winPanel = winPanelObj;
            losePanel = losePanelObj;
            winText = winPanel.GetComponentInChildren<TextMeshProUGUI>();
            loseText = losePanel.GetComponentInChildren<TextMeshProUGUI>();
        }
        
        // Find diamond count text
        GameObject diamondCountObj = GameObject.Find("DiamondCountText");
        if (diamondCountObj != null)
        {
            diamondCountText = diamondCountObj.GetComponent<TextMeshProUGUI>();
        }
        
        // Hide all panels and restart level
        StartLevel();
    }
} 