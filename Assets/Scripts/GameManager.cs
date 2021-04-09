using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PickableObject[] pickableObjects;
    [SerializeField] private Camera camera;
    [SerializeField] private Text helpText;

    public bool IsHelping = true;
    
    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        foreach (var pickableObject in pickableObjects)
            pickableObject.MainCamera = camera;
    }

    public void SetHelpText(string message)
    {
        helpText.text = message;
    }
}
