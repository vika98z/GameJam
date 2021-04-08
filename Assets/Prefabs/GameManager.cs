using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
