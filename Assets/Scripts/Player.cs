using UnityEngine;

public class Player : MonoBehaviour
{
  private const float GravityValue = -9.81f;
  
  [SerializeField] private float speed;
  [SerializeField] private float jumpHeight;
  [SerializeField] private float turnSmoothTime = 0.1f;
  [SerializeField] private Transform cam;
  [SerializeField] private float pushPower = 2f;

  private CharacterController _controller;

  private Vector3 _playerVelocity;
  private bool _groundedPlayer;
  private float _turnSmoothVelocity;

  private void Awake() =>
    _controller = GetComponent<CharacterController>();

  private void Update() =>
    Move();

  private void OnControllerColliderHit(ControllerColliderHit hit)
  {
    var body = hit.collider.attachedRigidbody;

    if (body == null || body.isKinematic || hit.moveDirection.y < -0.3)
      return;

    var pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
    body.velocity = pushDir * pushPower;
  }

  private void Move()
  {
    float vertical = Input.GetAxis("Vertical");
    float horizontal = Input.GetAxis("Horizontal");

    _groundedPlayer = _controller.isGrounded;
    if (_groundedPlayer && _playerVelocity.y < 0)
      _playerVelocity.y = 0f;

    Vector3 direction = new Vector3(horizontal, 0, vertical);

    if (direction.magnitude >= .1f)
    {
      float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
      float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, turnSmoothTime);
      transform.rotation = Quaternion.Euler(0f, angle, 0f);

      Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

      _controller.Move(moveDir.normalized * Time.deltaTime * speed);
    }

    if (Input.GetButtonDown("Jump") && _groundedPlayer)
      _playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * GravityValue);

    _playerVelocity.y += GravityValue * Time.deltaTime;
    _controller.Move(_playerVelocity * Time.deltaTime);
  }
}