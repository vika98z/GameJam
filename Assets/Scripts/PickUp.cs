using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
  [SerializeField] private new Camera camera;
  [SerializeField] private Transform objectHolder;
  [SerializeField] private PickableObject item;
  [SerializeField] private float pickUpDistance;

  private bool _carryObject;
  private PickableObject _highlightedObject;
  private List<PickableObject> _highlightedObjectsList = new List<PickableObject>();
  private bool _detectObject;
  private LayerMask _layerMask;

  private void Awake() =>
    _layerMask = ~ LayerMask.GetMask("Player");

  private void Update()
  {
    var directionRay = camera.ScreenPointToRay(Input.mousePosition);

    if (Physics.Raycast(directionRay, out var hit, pickUpDistance, _layerMask))
    {
      if (hit.collider.CompareTag("PickUp"))
      {
        var raycastedObject = hit.collider.gameObject.GetComponent<PickableObject>();
        if (raycastedObject != _highlightedObject)
        {
          _highlightedObject = raycastedObject;
          SetHighlighted();
        }
      }
      else
        ClearHighlighted();
    }
    else
      ClearHighlighted();

    if (CanPickUp())
      PickUpObject();
    else if (CanConnect())
      item.Connect(_highlightedObject);

    if (Input.GetMouseButton(0))
      Throw();

    bool CanPickUp() =>
      Input.GetKeyDown(KeyCode.E) && _detectObject && !_carryObject;

    bool CanConnect() =>
      Input.GetKeyDown(KeyCode.E) && _detectObject && _carryObject && item != _highlightedObject;
  }

  private void PickUpObject()
  {
    item = _highlightedObject;
    item.PickUp(objectHolder);
    _carryObject = true;
  }

  private void Throw()
  {
    if (item != null)
    {
      objectHolder.DetachChildren();
      item.Throw();
      _carryObject = false;
    }
  }

  private void SetHighlighted()
  {
    _detectObject = true;
    _highlightedObject.OutlineOn();
  }

  private void ClearHighlighted()
  {
    _detectObject = false;

    if (_highlightedObject != null)
    {
      _highlightedObject.OutlineOff();
      _highlightedObject = null;
    }
  }
}