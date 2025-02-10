using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UserInterfaceAmsterdam;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    public float levelStartTime { get; private set; }
    public float levelCompletionTime { get; private set; }
    public int diamondsCollected { get; private set; }

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI diamondCountText;
    
    [SerializeField] private UiPanelAnimationAndClickEvents _playButton;
    [SerializeField] private GameObject _uiPanelsContainer;
    [SerializeField] private GameObject _gamePanelsContainer;
    [SerializeField] private GameObject _gameEndPanel;
    [SerializeField] private TextMeshProUGUI _gameEndText;
    [SerializeField] private Image _gameEndBackground;  // Reference to panel background
    
    [Header("End Game Colors")]
    [SerializeField] private Color victoryColor = new Color(0, 0.6f, 0, 0.9f);  // Green tint
    [SerializeField] private Color gameOverColor = new Color(0.6f, 0, 0, 0.9f);  // Red tint

    private bool isGameOver = false;
    private bool canRestart = false;

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

        _playButton.OnClick += StartLevel;
        SceneManager.sceneLoaded += OnSceneLoaded;
        _uiPanelsContainer.SetActive(true);
        _gamePanelsContainer.SetActive(false);
        if (_gameEndPanel != null) _gameEndPanel.SetActive(false);
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
        if (canRestart && Input.GetKeyDown(KeyCode.Return))
        {
            RestartGame();
        }
    }

    private void OnDestroy()
    {
        _playButton.OnClick -= StartLevel;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void StartLevel()
    {
        _uiPanelsContainer.SetActive(false);
        _gamePanelsContainer.SetActive(true);
        if (_gameEndPanel != null) _gameEndPanel.SetActive(false);
        
        // Start the game
        Time.timeScale = 1f;
        levelStartTime = Time.time;
        isGameOver = false;
        canRestart = false;
        diamondsCollected = 0;
        UpdateDiamondUI();

        // Enable car sounds with a slight delay to ensure proper initialization
        StartCoroutine(EnableCarSoundsDelayed());
    }

    private IEnumerator EnableCarSoundsDelayed()
    {
        yield return new WaitForSeconds(0.1f);
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
        ShowGameEndPanel("Level Complete!", true);
        
        // Slow down time
        Time.timeScale = 0.3f;
    }

    public void GameOver()
    {
        if (isGameOver) return;
        
        isGameOver = true;
        levelCompletionTime = Time.time - levelStartTime;
        
        // Show game end panel
        ShowGameEndPanel("Game Over!", false);
        
        // Slow down time
        Time.timeScale = 0.3f;
    }

    private void ShowGameEndPanel(string headerText, bool isVictory)
    {
        if (_gameEndPanel != null && _gameEndText != null)
        {
            _gameEndPanel.SetActive(true);

            // Set appropriate background color
            if (_gameEndBackground != null)
            {
                _gameEndBackground.color = isVictory ? victoryColor : gameOverColor;
            }

            // Create the message with different content based on victory/game over
            string message;
            if (isVictory)
            {
                message = $"<size=60>{headerText}</size>\n\n" +
                         $"Congratulations!\n\n" +
                         $"Time: {levelCompletionTime:F2}s\n" +
                         $"Diamonds Collected: {diamondsCollected}";
            }
            else
            {
                message = $"<size=60>{headerText}</size>\n\n" +
                         $"Time Survived: {levelCompletionTime:F2}s\n" +
                         $"Diamonds Collected: {diamondsCollected}";
            }
            
            _gameEndText.text = message;
            
            // Enable restart after a short delay
            StartCoroutine(EnableRestart(1f, isVictory));
        }
    }

    private System.Collections.IEnumerator EnableRestart(float delay, bool isVictory)
    {
        yield return new WaitForSecondsRealtime(delay);
        canRestart = true;
        if (_gameEndText != null)
        {
            string restartText = isVictory ? 
                "\n\n<color=#FFEB04>Press ENTER to play again!</color>" : 
                "\n\n<color=#FFEB04>Press ENTER to try again!</color>";
            _gameEndText.text += restartText;
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
        
        // Show main menu
        if (_uiPanelsContainer != null) _uiPanelsContainer.SetActive(true);
        if (_gamePanelsContainer != null) _gamePanelsContainer.SetActive(false);
        if (_gameEndPanel != null) _gameEndPanel.SetActive(false);

        // Disable car sounds
        PrometeoCarController carController = FindObjectOfType<PrometeoCarController>();
        if (carController != null)
        {
            carController.useSounds = false;
        }
    }
} 