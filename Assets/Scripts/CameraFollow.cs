using System;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
  private const float YAngleMIN = 0.0f;
  private const float YAngleMAX = 50.0f;
  
  [SerializeField] private Transform target;
  [SerializeField] private Vector3 offset;
  [SerializeField] private float smoothTime = 0.3f;

  private Vector3 _velocity = Vector3.zero;

  private float _currentX;
  private float _currentY = 45.0f;
  private float speedRotateX = 1;
  private float speedRotateY = 1;  
  
  Vector3 playerPrevPos, playerMoveDir;
  float distance;
  
  private void Start()
  {
    playerPrevPos = target.transform.position; 
    distance = offset.magnitude;
  }

  private void Update()
  {
    _currentX += speedRotateX * Input.GetAxis("Mouse X");
    _currentY -= speedRotateY * Input.GetAxis("Mouse Y");

    _currentY = Mathf.Clamp(_currentY, YAngleMIN, YAngleMAX);
  }
  
  private void LateUpdate()
  {
    Vector3 targetPosition = target.position + offset;
    
    var rotation = new Vector3(_currentY, _currentX, 0);
    
    //_camTransform.position = position + rotation * dir;
    
    //transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, smoothTime);
    //transform.eulerAngles = rotation;
    //transform.LookAt(target);
    
    
    playerMoveDir = target.transform.position - playerPrevPos;
    playerMoveDir.Normalize();
    transform.position = target.transform.position - playerMoveDir * distance;

    transform.LookAt(target.transform.position);

    playerPrevPos = target.transform.position;
  }
}