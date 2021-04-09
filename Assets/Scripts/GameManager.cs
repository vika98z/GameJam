using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PickableObject[] pickableObjects;
    [SerializeField] private Camera camera;
    
    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        foreach (var pickableObject in pickableObjects)
            pickableObject.MainCamera = camera;
    }
}
