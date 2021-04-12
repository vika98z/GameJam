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

  [SerializeField] private GameObject pointLights;
  [SerializeField] private GameObject dirLight;

  [SerializeField] private GameObject bigPartOfCubes;
  [SerializeField] private GameObject staticCube;

  [SerializeField] private GameObject pauseMenu;

  [SerializeField] private GameObject loadingScreen;
  [SerializeField] private GameObject resultCanvas;
  [SerializeField] private Text resultText;
  
  [SerializeField] private GameObject GameOverPanel;
  [SerializeField] private GameObject StartHelpPanel;

  [SerializeField] private GameObject[] LegoCases;


  public bool IsHelping = true;

  private float _lightningXPosNormal = 140;
  private float _lightningXPosNight = -90;

  public bool GameIsPaused = false;

  private void Awake()
  {
    Cursor.visible = false;
    Cursor.lockState = CursorLockMode.Locked;
    Time.timeScale = 1f;

    RenderSettings.fog = false;
    

    foreach (var pickableObject in pickableObjects)
    {
      pickableObject.MainCamera = camera;
      pickableObject.Player = player;
    }

    player.OnButtonPressed += OnButtonPressed;
  }

  private void OnButtonPressed(int num)
  {
    Time.timeScale = 0f;
    GameIsPaused = true;

    Cursor.visible = true;
    Cursor.lockState = CursorLockMode.None;

    var level = PlayerPrefs.GetInt("Level");

    if (level == 0)
    {
      level = 2;
      PlayerPrefs.SetInt("Level", 2);
    }
    else if (level <= 3)
    {
      level++;
      PlayerPrefs.SetInt("Level", level);
    }
    else
    {
      GameOver();
      return;
    }

    PlayerPrefs.SetInt("Button", num);

    ResultOfLevel(level);

    PlayerPrefs.Save();
  }

  private void ResultOfLevel(int level)
  {
    var num = PlayerPrefs.GetInt("Button");
    resultCanvas.SetActive(true);
    switch (level)
    {
      case 2:
        switch (num)
        {
          //bad - blue
          case 0:
            resultText.text =
              "Ups, you didn’t guess. That’s why a thick fog descended on the room. But we believe in you! Next time you’re gonna be like!";
            break;
          //good - red
          case 1:
            resultText.text = "Wonderful! You made right choice! And as reward you get ability to jump 2 times higher!";
            break;
        }

        break;

      case 3:
        switch (num)
        {
          //good - blue
          case 0:
            resultText.text = "Wonderful! You made right choice! And as reward you get already prepared half of path!";
            break;
          //bad - red
          case 1:
            resultText.text =
              "Ups, you didn’t guess. That’s why night has fallen and you can see worse… But we believe in you! Next time you’re gonna be like!";
            break;
        }

        break;
        
      case 4:
        switch (num)
        {
          //good - blue
          case 0:
            resultText.text = "Wonderful! You made right choice! And as reward you get magnetic hands! On this level you can pull blocks that are much further away from you!";
            PlayerPrefs.SetInt("Distance", 30);
            break;
          //bad - red
          case 1:
            resultText.text =
              "Ups, you didn’t guess. That’s why you walk twice as slow. But we believe in you! Next time you’re gonna be like!";
            break;
        }

        break;
    }
  }

  public void Reload() => 
    StartCoroutine(LoadAsync("Room"));

  private void GameOver() => 
    GameOverPanel.SetActive(true);

  private void Start() =>
    LoadLevelSettings();

  //blue button = 0, red = 1
  private void LoadLevelSettings()
  {
    if (PlayerPrefs.GetInt("Level") == 0)
    {
      LegoCases[0].SetActive(true);
      SetStartHelp();
      return;
    }
    switch (PlayerPrefs.GetInt("Level"))
    {
      case 2:
        LegoCases[1].SetActive(true);

        switch (PlayerPrefs.GetInt("Button"))
        {
          //bad - blue
          case 0:
            RenderSettings.fog = true;
            break;
          //good - red
          case 1:
            player.SetBigJump();
            break;
        }

        break;

      case 3:
        LegoCases[2].SetActive(true);

        switch (PlayerPrefs.GetInt("Button"))
        {
          //good - blue
          case 0:
            bigPartOfCubes.SetActive(true);
            staticCube.SetActive(false);
            break;
          //bad - red
          case 1:
            pointLights.SetActive(false);
            var rotation = dirLight.transform.rotation;
            rotation = Quaternion.Euler(_lightningXPosNight, rotation.y, rotation.z);
            dirLight.transform.rotation = rotation;

            RenderSettings.fog = true;
            RenderSettings.fogColor = Color.black;
            RenderSettings.fogDensity = 0.09f;
            break;
        }

        break;

      case 4:
        LegoCases[3].SetActive(true);

        switch (PlayerPrefs.GetInt("Button"))
        {
          case 0:
            //player.Distance = PlayerPrefs.GetInt("Distance");
            break;
          case 1:
            player.speed = 3;
            break;
        }

        break;
    }
  }

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
      {
        Pause();
      }
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

  public void SetHelpText(string message)
  {
    helpText.text = message;
  }

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

  public void Quit()
  {
    Application.Quit();
  }
}