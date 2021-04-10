using UnityEngine;

public class ButtonController : MonoBehaviour
{
  [SerializeField] private AudioClip buttonSound;
  [SerializeField] private GameObject button;
  [SerializeField] private Player player;

  public int NumberOfButton;
  
  private AudioSource _audioSource;

  private bool _canPush = true;
  private bool _buttonHit;

  private float _buttonDownDistance = 0.16f;

  private void Start()
  {
    _audioSource = gameObject.AddComponent<AudioSource>();
    player.OnButtonPressed += _ => _buttonHit = true;
  }

  private void Update()
  {
    if (!_buttonHit || ! _canPush) return;

    if (buttonSound)
      _audioSource.PlayOneShot(buttonSound);

    _buttonHit = false;
    _canPush = false;

    var buttonPosition = button.transform.position;
    buttonPosition = new Vector3(buttonPosition.x, buttonPosition.y - _buttonDownDistance, buttonPosition.z);
    button.transform.position = buttonPosition;
  }
}