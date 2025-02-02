using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [Header("Menu Panels")]
    [SerializeField] private GameObject mainPanel;        // Main menu panel
    [SerializeField] private GameObject howToPlayPanel;   // How to play panel

    [Header("Menu Elements")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button howToPlayButton;
    [SerializeField] private Button backButton;           // Back button in how to play panel
    [SerializeField] private Button quitButton;

    [Header("Background")]
    [SerializeField] private Image backgroundImage;       // Reference to background image
    
    [Header("Scene Management")]
    [SerializeField] private string gameSceneName = "GameScene";  // Name of your game scene
    [SerializeField] private SceneTransition sceneTransition;     // Optional scene transition

    private void Start()
    {
        // Setup button listeners
        if (playButton != null)
            playButton.onClick.AddListener(StartGame);
            
        if (howToPlayButton != null)
            howToPlayButton.onClick.AddListener(ShowHowToPlay);
            
        if (backButton != null)
            backButton.onClick.AddListener(ShowMainMenu);
            
        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);

        // Show main menu at start
        ShowMainMenu();
    }

    public void StartGame()
    {
        if (sceneTransition != null)
        {
            // Use scene transition if available
            sceneTransition.LoadScene(gameSceneName);
        }
        else
        {
            // Direct scene load
            SceneManager.LoadScene(gameSceneName);
        }
    }

    public void ShowHowToPlay()
    {
        mainPanel.SetActive(false);
        howToPlayPanel.SetActive(true);
    }

    public void ShowMainMenu()
    {
        mainPanel.SetActive(true);
        howToPlayPanel.SetActive(false);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
} 