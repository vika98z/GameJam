using UnityEngine;

public enum DetailType { Cube, Rectangle};

public class PickableObject : MonoBehaviour
{
  [SerializeField] private GameObject projectionCubePrefab;
  
  public bool IsConnectable;
  public bool IsHighlightable;
  public DetailType TypeOfDetail;
  
  private Rigidbody _rigidbody;
  private Outline _outlineScript;
  private bool _picked;
  private BoxCollider _collider;

  public bool IsConnected;

  public GameObject _projection;
  
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

  public void OutlineOff()
  {
    _outlineScript.enabled = false;
    ProjectionOff();
  }

  private void ProjectionOff()
  {
    //if (_projection != null)
      Destroy(_projection);
  }

  public bool Connect(PickableObject other)
  {
    if (TypeOfDetail == DetailType.Cube && other.TypeOfDetail == DetailType.Cube)
    {
      transform.position = other.transform.position + new Vector3(0, other._collider.size.y / 2 + _collider.size.y / 2, 0);
      transform.forward = other.transform.forward;
      IsConnected = true;

      ProjectionOff();

      other.ConnectFromOther();
      
      return true;
    }

    return false;
  }

  private void ConnectFromOther()
  {
    ProjectionOff();

    IsConnected = true;
    IsHighlightable = false;
    _rigidbody.isKinematic = true;
  }

  public void CreateProjection(PickableObject item)
  {
    if (TypeOfDetail == DetailType.Cube && item.TypeOfDetail == DetailType.Cube)
    {
      if (_projection == null)
      {
        var prefabCollider = projectionCubePrefab.GetComponent<BoxCollider>();
        var pos = transform.position + new Vector3(0, _collider.size.y / 2 + prefabCollider.size.y / 2, 0);
        _projection = Instantiate(projectionCubePrefab, pos, Quaternion.identity);
      
        _projection.transform.forward = transform.forward;
      }
    }
  }
}