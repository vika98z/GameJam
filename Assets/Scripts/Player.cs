using System;
using UnityEngine;

public class Player : MonoBehaviour
{
  private const float GravityValue = -9.81f;

  [SerializeField] public float speed;
  [SerializeField] private float jumpHeight;
  [SerializeField] private float turnSmoothTime = 0.1f;
  [SerializeField] private Transform cam;
  [SerializeField] private float pushPower = 2f;

  [SerializeField] private Animator animator;
  [SerializeField] private float startAnimTime = 0.3f;
  [SerializeField] private float stopAnimTime = 0.15f;

  [SerializeField] private AudioClip jumpClip;
  
  public Action<int> OnButtonPressed;

  private float Distance = 1.5f;

  private CharacterController _controller;

  private Vector3 _playerVelocity;
  private bool _groundedPlayer;
  private float _turnSmoothVelocity;
  private readonly float allowPlayerRotation = 0.1f;
  private AudioSource _audioSource;
  private Vector3 moveDir;
  
  private void Awake()
  {
    _controller = GetComponent<CharacterController>();
    _audioSource = GetComponent<AudioSource>();
  }

  private void Start()
  {
    var dist = PlayerPrefs.GetInt("Distance");
    if (dist > 0)
      Distance = dist;
    else
    {
      Distance = 1.5f;
    }
  }

  private void Update() =>
    Move();

  private void OnControllerColliderHit(ControllerColliderHit hit)
  {
    if (hit.collider.gameObject.CompareTag("Button"))
    {
      var gameObj = hit.collider.gameObject;
      var butCtrl = gameObj.GetComponent<ButtonController>();
      OnButtonPressed?.Invoke(butCtrl.NumberOfButton);
      return;
    }
    
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

    SetAnimation(horizontal, vertical);

    Vector3 direction = new Vector3(horizontal, 0, vertical);

    if (direction.magnitude >= .1f)
    {
      float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
      float angle =
        Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, turnSmoothTime);
      transform.rotation = Quaternion.Euler(0f, angle, 0f);

      moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

      //_controller.Move(moveDir.normalized * Time.deltaTime * speed);
    }
    else
    {
      moveDir=Vector3.zero;
    }

    if (Input.GetButtonDown("Jump") && _groundedPlayer)
    {
      _playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * GravityValue);
      _audioSource.PlayOneShot(jumpClip);
    }

    _playerVelocity.y += GravityValue * Time.deltaTime;
    _controller.Move(moveDir.normalized * Time.deltaTime * speed + _playerVelocity * Time.deltaTime);
  }

  private void SetAnimation(float horizontal, float vertical)
  {
    var animSpeed = new Vector2(horizontal, vertical).sqrMagnitude;

    if (_groundedPlayer)
    {
      if (animSpeed > allowPlayerRotation)
        animator.SetFloat("Blend", animSpeed, startAnimTime, Time.deltaTime);
      else if (animSpeed < allowPlayerRotation)
        animator.SetFloat("Blend", animSpeed, stopAnimTime, Time.deltaTime);
    }
    else
      animator.SetFloat("Blend", 0, startAnimTime, Time.deltaTime);
  }

  public void SetBigJump() => 
    jumpHeight = 3.5f;
}