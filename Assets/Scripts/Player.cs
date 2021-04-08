using UnityEngine; 
 
public class Player : MonoBehaviour 
{ 
  [SerializeField] private float speed; 
  [SerializeField] private float turnSpeed; 
  [SerializeField] private float gravity;
  [SerializeField] private float jumpHeight;
  
  private CharacterController _controller; 
  private Vector3 _moveDirection = Vector3.zero; 
 
  private Vector3 _playerVelocity;
  private bool _groundedPlayer;
  private float gravityValue = -9.81f;
  
  private void Awake() => 
    _controller = GetComponent<CharacterController>(); 
 
  private void Update() => 
    Move(); 
 
  private void Move() 
  { 
    float vertical = Input.GetAxis("Vertical"); 
    float horizontal = Input.GetAxis("Horizontal");

    _groundedPlayer = _controller.isGrounded;
    if (_groundedPlayer && _playerVelocity.y < 0)
      _playerVelocity.y = 0f;

    Vector3 move = new Vector3(horizontal, 0, vertical);
    _controller.Move(move * Time.deltaTime * speed);

    if (move != Vector3.zero)
      gameObject.transform.forward = move;

    if (Input.GetButtonDown("Jump") && _groundedPlayer)
      _playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);

    _playerVelocity.y += gravityValue * Time.deltaTime;
    _controller.Move(_playerVelocity * Time.deltaTime);
  } 
}