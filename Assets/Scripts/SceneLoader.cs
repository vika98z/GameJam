using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string loadLevel;

    [SerializeField] private GameObject loadingScreen;
    
    public void Load()
    {
        PlayerPrefs.DeleteAll();
        loadingScreen.SetActive(true);
        StartCoroutine(LoadAsync());
    }

    private IEnumerator LoadAsync()
    {
        var asyncOperation = SceneManager.LoadSceneAsync(loadLevel);

        while (!asyncOperation.isDone)
            yield return null;
    }
    
    public void Quit()
    {
        Application.Quit();
    }
}
