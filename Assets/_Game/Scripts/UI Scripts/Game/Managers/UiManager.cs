using UnityEngine;
using UserInterfaceAmsterdam;

public class UiManager : MonoBehaviour
{
   [SerializeField] private UiPanelAnimationAndClickEvents _playButton;
   [SerializeField] private UiPanelAnimationAndClickEvents _quitButton;

   [SerializeField] private GameObject _camera;
   [SerializeField] private GameObject _ui;
   
   private GameObject _canvasSpawned;

   private void Awake()
   {
      _playButton.OnClick += EnterPlayerMode;
      _quitButton.OnClick += OnQuitButtonPressed;
      EscapeButtonListener.OnEscapeButtonPressed += ExitPlayMode;
      ExitPlayMode();
   }

   private void OnDestroy()
   {
      _playButton.OnClick -= EnterPlayerMode;
      _quitButton.OnClick -= OnQuitButtonPressed;
      EscapeButtonListener.OnEscapeButtonPressed -= ExitPlayMode;
   }

   private void CacheCanvas(GameObject obj) => _canvasSpawned = obj; 

   private void EnterPlayerMode()
   {
      _ui.SetActive(false);
      _camera.SetActive(false);
      _canvasSpawned.SetActive(true);
   }

   private void ExitPlayMode()
   {
      _ui.SetActive(true);
      _camera.SetActive(true);
      if(_canvasSpawned)_canvasSpawned.SetActive(false);
   }

   private void OnQuitButtonPressed()
   {
      Application.Quit();
   }
}
