using System;
using UnityEngine;
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
  
  public bool IsHelping = true;

  private float _lightningXPosNormal = 140;
  private float _lightningXPosNight = -90;
  
  private void Awake()
  {
    Cursor.visible = false;
    Cursor.lockState = CursorLockMode.Locked;
    
    RenderSettings.fog = false;

    foreach (var pickableObject in pickableObjects)
    {
      pickableObject.MainCamera = camera;
      pickableObject.Player = player;
    }
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
            RenderSettings.fog = false;
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

  public void SetHelpText(string message)
  {
    helpText.text = message;
  }
}