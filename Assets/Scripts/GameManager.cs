﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
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
      PlayerPrefs.SetInt("Level", 2);
    else if (level < 3)
    {
      level++;
      PlayerPrefs.SetInt("Level", level);
    }
    else
      GameOver();

    PlayerPrefs.SetInt("Button", num);

    ResultOfLevel(level, num);

    PlayerPrefs.Save();
  }

  private void ResultOfLevel(int level, int num)
  {
    resultCanvas.SetActive(true);
    switch (level)
    {
      case 2:
        switch (num)
        {
          //bad - blue
          case 0:
            //RenderSettings.fog = true;
            resultText.text =
              "Ups, you didn’t guess. That’s why a thick fog descended on the room. But we believe in you! Next time you’re gonna be like!";
            break;
          //good - red
          case 1:
            //player.SetBigJump();
            resultText.text = "Wonderful! You made right choice! And as reward you get ability to jump 2 times higher!";
            break;
        }

        break;

      case 3:
        switch (num)
        {
          //good - blue
          case 0:
            //bigPartOfCubes.SetActive(true);
            //staticCube.SetActive(false);
            resultText.text = "Wonderful! You made right choice! And as reward you get already prepared half of path!";
            break;
          //bad - red
          case 1:
            //pointLights.SetActive(false);
            //var rotation = dirLight.transform.rotation;
            //rotation = Quaternion.Euler(_lightningXPosNight, rotation.y, rotation.z);
            //dirLight.transform.rotation = rotation;
            resultText.text =
              "Ups, you didn’t guess. That’s why 4.	night has fallen and you can see worse… But we believe in you! Next time you’re gonna be like!";

            break;
        }

        break;
    }
  }

  public void Reload()
  {
    StartCoroutine(LoadAsync("Room"));
  }

  private void GameOver()
  {
  }

  private void Start() =>
    LoadLevelSettings();

  //blue button = 0, red = 1
  private void LoadLevelSettings()
  {
    switch (PlayerPrefs.GetInt("Level"))
    {
      case 2:
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
            break;
        }

        break;

      case 4:
        switch (PlayerPrefs.GetInt("Button"))
        {
          case 0:
            break;
          case 1:
            break;
        }

        break;

      case 5:
        switch (PlayerPrefs.GetInt("Button"))
        {
          case 0:
            break;
          case 1:
            break;
        }

        break;
    }
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
      else
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