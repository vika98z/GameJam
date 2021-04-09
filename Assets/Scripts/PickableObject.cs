using System;
using UnityEngine;

public enum DetailType
{
  Cube,
  Rectangle
};

public class PickableObject : MonoBehaviour
{
  [SerializeField] private GameObject projectionCubePrefab;
  [SerializeField] private float pickUpDistance;
  [SerializeField] private GameObject[] parts;
  
  public Camera MainCamera { get; set; }
  public bool IsConnectable;
  public bool IsHighlightable;
  public DetailType TypeOfDetail;

  private Rigidbody _rigidbody;
  private Outline _outlineScript;
  private bool _picked;
  private BoxCollider _collider;

  public bool IsConnected;

  public GameObject _projection;

  public int Sign = 1;
  public float tempSign = 1;
  
  //для установки квадрата на прямоугольник
  private bool checkSideOfRecktangle;
  private LayerMask _layerMask;
  public Vector3 _rectanglePartPosition;
  

  private void Awake()
  {
    _rigidbody = GetComponent<Rigidbody>();
    _outlineScript = GetComponent<Outline>();
    _collider = GetComponent<BoxCollider>();

    _rectanglePartPosition = parts[0].transform.position;
    
    //_layerMask = ~ LayerMask.GetMask("Player");
    _layerMask = LayerMask.GetMask("LegoPart");
  }

  private void Update()
  {
    if (_picked)
    {
      if (Input.GetAxis("Mouse ScrollWheel") > 0)
      {
        tempSign += .5f;
      }

      if (Input.GetAxis("Mouse ScrollWheel") < 0)
      {
        tempSign -= .5f;
      }

      tempSign = Mathf.Clamp(tempSign, -1, 1);
    }

    if (checkSideOfRecktangle)
    {
      var directionRay = MainCamera.ScreenPointToRay(Input.mousePosition);

      if (Physics.Raycast(directionRay, out var hit, pickUpDistance, _layerMask))
      {
        if (hit.collider.CompareTag("RectanglePart"))
          _rectanglePartPosition = hit.collider.gameObject.transform.position;
      }
    }
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
    checkSideOfRecktangle = false;
    Destroy(_projection);
  }

  public bool Connect(PickableObject other)
  {
    //ставим квадрат на квадрат 
    if (TypeOfDetail == DetailType.Cube && other.TypeOfDetail == DetailType.Cube)
    {
      transform.position = other.transform.position +
                           new Vector3(0, other._collider.size.y / 2 + _collider.size.y / 2, 0);
      transform.forward = other.transform.forward;
      IsConnected = true;

      ProjectionOff();

      other.ConnectFromOther();

      return true;
    }

    //ставим квадрат на прямоугольник. проверка в квадрате - значит берем позицию установки из прямоугольника
    if (TypeOfDetail == DetailType.Cube && other.TypeOfDetail == DetailType.Rectangle)
    {
      transform.position = other._rectanglePartPosition + new Vector3(0, _collider.size.y, 0);
      //other.transform.position + new Vector3(Sign * _collider.size.x / 2,
        //other._collider.size.y / 2 + _collider.size.y / 2, 0);
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
    //квадрат ставим на квадрат
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
    //квадрат ставим на прямоугольник. проверка в прямоугольнике!
    else if (TypeOfDetail == DetailType.Rectangle && item.TypeOfDetail == DetailType.Cube)
    {
      if (_projection == null)
      {
        checkSideOfRecktangle = true;
        // //1 вариант
        //
        var prefabCollider = projectionCubePrefab.GetComponent<BoxCollider>();
        var pos = _rectanglePartPosition + new Vector3(0, prefabCollider.size.y, 0);
          //transform.position + new Vector3(prefabCollider.size.x / 2, _collider.size.y / 2 + prefabCollider.size.y / 2, 0);
        _projection = Instantiate(projectionCubePrefab, pos, Quaternion.identity);
        
        _projection.transform.forward = transform.forward;
      }
    }
  }
}