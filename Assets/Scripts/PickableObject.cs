using UnityEngine;

public class PickableObject : MonoBehaviour
{
  private Rigidbody _rigidbody;
  private Outline _outlineScript;
  private bool _picked;
  private Collider _collider;
  
  private void Awake()
  {
    _rigidbody = GetComponent<Rigidbody>();
    _outlineScript = GetComponent<Outline>();
    _collider = GetComponent<Collider>();
  }

  private void Start() =>
    OutlineOff();

  public void PickUp(Transform holdObject)
  {
    _collider.isTrigger = true;
    
    transform.SetParent(holdObject);
    transform.position = holdObject.position;
    transform.forward = holdObject.forward;
    
    _rigidbody.isKinematic = true;
    _rigidbody.useGravity = false;
    
    _picked = true;
    
    OutlineOff();
  }
  
  public void Throw()
  {
    _collider.isTrigger = false;

    _rigidbody.isKinematic = false;
    _rigidbody.useGravity = true;
    _picked = false;
  }

  public void OutlineOn()
  {
    if (!_picked)
      _outlineScript.enabled = true;
  }

  public void OutlineOff() => 
    _outlineScript.enabled = false;

  public void Connect(PickableObject other)
  {
    print("connect");
  }
}