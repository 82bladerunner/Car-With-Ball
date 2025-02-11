using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    public float levelStartTime { get; private set; }
    public float levelCompletionTime { get; private set; }
    public int diamondsCollected { get; private set; }

    [Header("UI Elements")] 
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI diamondCountText;
    [SerializeField] private List<Button> _quitButtons;
    [SerializeField] private List<Button> _restartButtons;
    [SerializeField] private List<Button> _playButtons;
    [SerializeField] private Button _giveFeedbackButton;
    
    [SerializeField] private GameObject _winPanel;
    [SerializeField] private GameObject _losePanel;
    [SerializeField] private GameObject _mainMenuPanel;
    [SerializeField] private GameObject _welcomePanel;
    
    [SerializeField] private TextMeshProUGUI _winPanelText;
    [SerializeField] private TextMeshProUGUI _losePanelText;
    
    private bool isGameOver = false;
    private bool canRestart = false;
    private bool isPaused = false;

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

        // Freeze the game at start
        Time.timeScale = 0f;

        // Make sure car sounds are off at start
        PrometeoCarController carController = FindObjectOfType<PrometeoCarController>();
        if (carController != null)
        {
            carController.useSounds = false;
        }

        foreach (var quitButton in _quitButtons)
        {
            quitButton.onClick.AddListener(() =>
            {
                Application.Quit();
            });
        }
        
        foreach (var restartButton in _restartButtons)
        {
            restartButton.onClick.AddListener(() =>
            {
                RestartGame();
            });
        }
        
        foreach (var playButton in _playButtons)
        {
            playButton.onClick.AddListener(() =>
            {
                _mainMenuPanel.SetActive(false);
                StartLevel();
            });
        }
        
        _welcomePanel.SetActive(true);
        SceneManager.sceneLoaded += OnSceneLoaded;
        _giveFeedbackButton.onClick.AddListener(() =>
        {
            Application.OpenURL("https://google.com");
        });
    }
    
    private void Update()
    {
        // Update timer only if game is not over
        if (timerText != null && !isGameOver)
        {
            float currentTime = Time.time - levelStartTime;
            timerText.text = $"Time: {currentTime:F2}";
        }

        // Check for restart input when game is over
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ShowMainMenu();
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void ShowMainMenu()
    {
        _mainMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    private void StartLevel()
    {
        // Start the game
        Time.timeScale = 1f;
        if (!isPaused)
        {
            levelStartTime = Time.time;
            diamondsCollected = 0;
        }
        isGameOver = false;
        canRestart = false;
        isPaused = false;
        UpdateDiamondUI();

        // Enable car controller and sounds
        PrometeoCarController carController = FindObjectOfType<PrometeoCarController>();
        if (carController != null)
        {
            carController.useSounds = true;
        }
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
        
        // Show game end panel
        ShowGameEndPanel(true);
        
        // Slow down time
        Time.timeScale = 0.3f;
    }

    public void GameOver()
    {
        if (isGameOver) return;
        
        isGameOver = true;
        levelCompletionTime = Time.time - levelStartTime;
        
        // Show game end panel
        ShowGameEndPanel(false);
        
        // Slow down time
        Time.timeScale = 0.3f;
    }

    private void ShowGameEndPanel(bool isVictory)
    {
        // Create the message with different content based on victory/game over
        string message;
        if (isVictory)
        {
            message = $"You successfully delivered the beach ball to its destination. Precision, control, and skill paid off.\n\nReady for another challenge?\n" +
                      $"Time: {levelCompletionTime:F2}s\n" +
                      $"Diamonds Collected: {diamondsCollected}";
            _winPanel.SetActive(true);
            _winPanelText.text = message;
        }
        else
        {
            message = $"The ball has fallen, and the challenge is over this time. Stay sharp, adjust your strategy, and try again.\n\nYou can do this!\n" +
                      $"Time Survived: {levelCompletionTime:F2}s\n" +
                      $"Diamonds Collected: {diamondsCollected}";
            _losePanel.SetActive(true);
            _losePanelText.text = message;
        }

    }

    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Freeze game on scene load
        Time.timeScale = 0f;
        timerText.text = "Time: 0.00";
        
        // Disable car sounds
        PrometeoCarController carController = FindObjectOfType<PrometeoCarController>();
        if (carController != null)
        {
            carController.useSounds = false;
        }
    }
} 