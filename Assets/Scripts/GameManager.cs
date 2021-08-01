using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
  [SerializeField] private PickableObject[] pickableObjects;
  [SerializeField] private Camera camera;
  [SerializeField] private Player player;
  [SerializeField] private Text helpText;

  [SerializeField] private GameObject pauseMenu;

  [SerializeField] private GameObject loadingScreen;
  
  [SerializeField] private GameObject StartHelpPanel;

  private float _lightningXPosNormal = 140;
  private float _lightningXPosNight = -90;

  public bool GameIsPaused;

  private void Awake()
  {
    Cursor.visible = false;
    Cursor.lockState = CursorLockMode.Locked;
    Time.timeScale = 1f;

    foreach (var pickableObject in pickableObjects)
    {
      pickableObject.MainCamera = camera;
      pickableObject.Player = player;
    }
  }

  public void Reload() => 
    StartCoroutine(LoadAsync("Room"));

  private void Start() =>
    SetStartHelp();

  private void SetStartHelp()
  {
    Time.timeScale = 0f;
    Cursor.visible = true;
    Cursor.lockState = CursorLockMode.None;
    StartHelpPanel.SetActive(true);
  }

  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.Escape))
    {
      if (GameIsPaused)
      {
        if (pauseMenu.activeSelf)
          Resume();
      }
      else if (Time.timeScale > 0.5f)
        Pause();
    }
  }

  private void Pause()
  {
    pauseMenu.SetActive(true);
    Time.timeScale = 0f;
    GameIsPaused = true;

    Cursor.visible = true;
    Cursor.lockState = CursorLockMode.None;
  }

  public void Resume()
  {
    pauseMenu.SetActive(false);
    Time.timeScale = 1f;
    GameIsPaused = false;

    Cursor.visible = false;
    Cursor.lockState = CursorLockMode.Locked;
  }

  public void SetHelpText(string message) => 
    helpText.text = message;

  public void MainMenu()
  {
    loadingScreen.SetActive(true);
    StartCoroutine(LoadAsync("Menu"));
  }

  private IEnumerator LoadAsync(string name)
  {
    var asyncOperation = SceneManager.LoadSceneAsync(name);

    while (!asyncOperation.isDone)
      yield return null;
  }

  public void Quit() => 
    Application.Quit();
}