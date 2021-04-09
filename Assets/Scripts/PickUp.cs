using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PickUp : MonoBehaviour
{
  [SerializeField] private new Camera camera;
  [SerializeField] private Transform objectHolder;
  [SerializeField] private PickableObject item;
  [SerializeField] private float pickUpDistance;

  private bool _carryObject;

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
        if (_highlightedObjectsList.Count > 0)
        {
          if (!_highlightedObjectsList.Contains(raycastedObject))
          {
            _highlightedObjectsList.Add(raycastedObject);
            SetHighlighted();
          }
        }
        else
        {
          _highlightedObjectsList.Add(raycastedObject);
          SetHighlighted();
        }
      }
      else
        ClearHighlighted();
    }
    else
      ClearHighlighted();

    if (CanPickUp())
    {
      if (!_highlightedObjectsList.Last().IsConnected && _highlightedObjectsList.Last().IsHighlightable)
        PickUpObject();
    }
    else if (CanConnect())
    {
      if (item.Connect(_highlightedObjectsList.Last()))
        Throw();
      return;
    }

    if (Input.GetMouseButton(0))
      Throw();

    bool CanPickUp() =>
      Input.GetKeyDown(KeyCode.E) && _detectObject && !_carryObject;

    bool CanConnect() =>
      item != null && _highlightedObjectsList.Count > 0 &&  item.IsConnectable && Input.GetMouseButton(0) && _detectObject && _carryObject &&
      item != _highlightedObjectsList.Last() && _highlightedObjectsList.Last().IsHighlightable;
  }

  private void PickUpObject()
  {
    item = _highlightedObjectsList.Last();
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

      item = null;
    }
  }

  private void SetHighlighted()
  {
    _detectObject = true;
    var curObjectToHighlight = _highlightedObjectsList.Last();
    if (curObjectToHighlight)
    {
      curObjectToHighlight.OutlineOn();

      if (item != null && curObjectToHighlight.IsConnected && curObjectToHighlight.IsHighlightable)
      {
        curObjectToHighlight.CreateProjection(item);
      }
    }
  }

  private void ClearHighlighted()
  {
    _detectObject = false;

    if (_highlightedObjectsList.Count > 0)
    {
      _highlightedObjectsList.ForEach(p => p?.OutlineOff());
    }

    _highlightedObjectsList.Clear();
    _highlightedObjectsList = new List<PickableObject>();
  }
}