using UnityEngine;

public enum DetailType { Cube, Rectangle};

public class PickableObject : MonoBehaviour
{
  public bool IsConnectable;
  public bool IsHighlightable;
  public DetailType TypeOfDetail;
  
  private Rigidbody _rigidbody;
  private Outline _outlineScript;
  private bool _picked;
  private BoxCollider _collider;

  public bool IsConnected;
  
  private void Awake()
  {
    _rigidbody = GetComponent<Rigidbody>();
    _outlineScript = GetComponent<Outline>();
    _collider = GetComponent<BoxCollider>();
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

    if (!IsConnected)
    {
      _rigidbody.isKinematic = false;
      _rigidbody.useGravity = true;
    }

    _picked = false;
  }

  public void OutlineOn()
  {
    if (!_picked && IsHighlightable)
      _outlineScript.enabled = true;
  }

  public void OutlineOff() => 
    _outlineScript.enabled = false;

  public bool Connect(PickableObject other)
  {
    if (TypeOfDetail == DetailType.Cube && other.TypeOfDetail == DetailType.Cube)
    {
      transform.position = other.transform.position + new Vector3(0, other._collider.size.y / 2 + _collider.size.y / 2, 0);//other.transform.up * transform.localScale.y;
      transform.forward = other.transform.forward;
      IsConnected = true;
      other.IsConnected = true;
      other.IsHighlightable = false;
      other._rigidbody.isKinematic = true;
      return true;
    }

    return false;
  }
}