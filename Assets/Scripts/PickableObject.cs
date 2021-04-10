using UnityEngine;

public enum DetailType
{
  Cube,
  Rectangle
};

public class PickableObject : MonoBehaviour
{
  [SerializeField] private GameObject projectionCubePrefab;
  [SerializeField] private GameObject projectionRectPrefab;
  [SerializeField] private float pickUpDistance;
  [SerializeField] private GameObject[] parts;

  public Camera MainCamera { get; set; }
  public Player Player { get; set; }
  
  public bool IsConnectable;
  public bool IsHighlightable;
  public DetailType TypeOfDetail;

  private Rigidbody _rigidbody;
  private Outline _outlineScript;
  private bool _picked;
  private BoxCollider _collider;

  public bool IsConnected;

  public GameObject _projection;

  public int Angle = 0;

  //для установки квадрата на прямоугольник
  private bool checkSideOfRecktangle;
  private LayerMask _layerMask;

  public Vector3 _rectanglePartPosition;

  //сохраним на время ссылку на объекта в руках, чтобы перерисовывать проекцию
  private PickableObject _itemForProjection;

  //для установки прямоугольника на что-либо
  private bool _canRotate;
  public Vector3 _projectionRotation;

  private void Awake()
  {
    _rigidbody = GetComponent<Rigidbody>();
    _outlineScript = GetComponent<Outline>();
    _collider = GetComponent<BoxCollider>();

    _rectanglePartPosition = parts[0].transform.position;

    _layerMask = LayerMask.GetMask("LegoPart");
  }

  private void Update()
  {
    if (Input.GetAxis("Mouse ScrollWheel") > 0)
    {
      Angle += 45;
      if (Angle % 90 == 0)
        ChangeProjectionRotation(new Vector3(0, Angle, 0));
    }

    if (Input.GetAxis("Mouse ScrollWheel") < 0)
    {
      Angle -= 45;
      if (Angle % 90 == 0)
        ChangeProjectionRotation(new Vector3(0, Angle, 0));
    }

    if (checkSideOfRecktangle)
    {
      var directionRay = MainCamera.ScreenPointToRay(Input.mousePosition);

      var distance = pickUpDistance + 
                     Vector3.Distance(Player.transform.position, MainCamera.transform.position);
      
      if (Physics.Raycast(directionRay, out var hit, distance, _layerMask))
      {
        if (hit.collider.CompareTag("RectanglePart") && hit.collider.gameObject.transform.parent.parent.GetComponent<PickableObject>().IsConnected)
        {
          var oldPos = _rectanglePartPosition;
          _rectanglePartPosition = hit.collider.gameObject.transform.position;
          if (oldPos != _rectanglePartPosition)
          {
            if (!_canRotate)
              ChangeProjectionPosition(_rectanglePartPosition, projectionCubePrefab);
            else
              ChangeProjectionPosition(_rectanglePartPosition, projectionRectPrefab);
          }
        }
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
    checkSideOfRecktangle = _canRotate = false;
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

    //ставим прямоугольник на квадрат. проверка в прямоугольнике - значит берем позицию установки из квадрата
    if (TypeOfDetail == DetailType.Rectangle && other.TypeOfDetail == DetailType.Cube)
    {
      transform.position = other.transform.position + new Vector3(0, _collider.size.y, 0);
      transform.rotation = Quaternion.Euler(other._projectionRotation);
      IsConnected = true;

      ProjectionOff();

      other.ConnectFromOther();

      return true;
    }
    
    //ставим прямоугольник на прямоугольник
    if (TypeOfDetail == DetailType.Rectangle && other.TypeOfDetail == DetailType.Rectangle)
    {
      transform.position = other._rectanglePartPosition + new Vector3(0, _collider.size.y, 0);
      transform.rotation = Quaternion.Euler(other._projectionRotation);
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
    _itemForProjection = item;

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

    //прямоугольник ставим на квадрат. проверка в квадрате!
    else if (TypeOfDetail == DetailType.Cube && item.TypeOfDetail == DetailType.Rectangle)
    {
      _canRotate = true;
      if (_projection == null)
      {
        var prefabCollider = projectionRectPrefab.GetComponent<BoxCollider>();
        var pos = transform.position + new Vector3(0, prefabCollider.size.y, 0);
        //transform.position + new Vector3(prefabCollider.size.x / 2, _collider.size.y / 2 + prefabCollider.size.y / 2, 0);
        _projection = Instantiate(projectionRectPrefab, pos, Quaternion.identity);

        _projection.transform.forward = transform.forward;
      }
    }
    
    //прямоугольник ставим на прямоугольник
    else if (TypeOfDetail == DetailType.Rectangle && item.TypeOfDetail == DetailType.Rectangle)
    {
      _canRotate = true;
      if (_projection == null)
      {
        checkSideOfRecktangle = true;
        
        var prefabCollider = projectionRectPrefab.GetComponent<BoxCollider>();
        var pos = _rectanglePartPosition + new Vector3(0, prefabCollider.size.y, 0);
        //transform.position + new Vector3(prefabCollider.size.x / 2, _collider.size.y / 2 + prefabCollider.size.y / 2, 0);
        _projection = Instantiate(projectionRectPrefab, pos, Quaternion.identity);

        _projection.transform.forward = transform.forward;
      }
    }
  }

  private void ChangeProjectionPosition(Vector3 newpos, GameObject prefab)
  {
    if (_projection != null)
    {
      var prefabCollider = prefab.GetComponent<BoxCollider>();
      _projection.transform.position = newpos + new Vector3(0, prefabCollider.size.y, 0);
    }
  }

  private void ChangeProjectionRotation(Vector3 newrot)
  {
    if (_projection != null && _canRotate)
    {
      _projection.transform.rotation = Quaternion.Euler(newrot);
      _projectionRotation = newrot;
    }
  }
}