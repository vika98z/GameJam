using UnityEngine;

public class PickUp : MonoBehaviour
{
  [SerializeField] private new Camera camera;
  [SerializeField] private Transform objectHolder;
  [SerializeField] private PickableObject item;
  [SerializeField] private float pickUpDistance = 2f;

  private bool _carryObject;
  private PickableObject _lastHighlightedObject;
  private bool _detectObject;
  
  void Update()
  {
    var directionRay = new Ray(transform.position, camera.transform.forward);
    
    if (Physics.Raycast(directionRay, out var hit, pickUpDistance))
    {
      if (hit.collider.CompareTag("PickUp"))
      {
        _lastHighlightedObject = hit.collider.gameObject.GetComponent<PickableObject>();
        SetHighlighted();
      }
      else
        ClearHighlighted();
    }

    if (Input.GetKeyDown(KeyCode.E) && _detectObject)
    {
      item = _lastHighlightedObject;
      item.PickUp(objectHolder);
    }
    
    if (Input.GetMouseButton(1))
    {
      if (item != null)
      {
        objectHolder.DetachChildren();
        item.Throw();
      }
    }
  }

  private void SetHighlighted()
  {
    _detectObject = true;
    _lastHighlightedObject.OutlineOn();
  }
  
  private void ClearHighlighted()
  {
    _detectObject = false;

    if (_lastHighlightedObject != null)
    {
      _lastHighlightedObject.OutlineOff();
      _lastHighlightedObject = null;
    }
  }
}