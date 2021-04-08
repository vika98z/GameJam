using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PickUp : MonoBehaviour
{
  [SerializeField] private new Camera camera;
  [SerializeField] private Transform objectHolder;
  [SerializeField] private PickableObject item;
  [SerializeField] private float pickUpDistance = 2f;

  private bool _carryObject;
  private List<PickableObject> _highlightedObjects = new List<PickableObject>();
  private bool _detectObject;

  private void Update()
  {
    var directionRay = new Ray(transform.position, camera.transform.forward);
    
    if (Physics.Raycast(directionRay, out var hit, pickUpDistance))
    {
      if (hit.collider.CompareTag("PickUp"))
      {
        _highlightedObjects.Add(hit.collider.gameObject.GetComponent<PickableObject>());
        SetHighlighted();
      }
      else
        ClearHighlighted();
    }
    else
      ClearHighlighted();
    
    if (CanPickUp())
    {
      item = _highlightedObjects.First();
      item.PickUp(objectHolder);
      _carryObject = true;
    }
    
    if (Input.GetMouseButton(0))
    {
      if (item != null)
      {
        objectHolder.DetachChildren();
        item.Throw();
        _carryObject = false;
      }
    }

    bool CanPickUp() => 
      Input.GetKeyDown(KeyCode.E) && _detectObject && !_carryObject;
  }

  private void SetHighlighted()
  {
    _detectObject = true;
    _highlightedObjects.ForEach(p => p.OutlineOn());
  }
  
  private void ClearHighlighted()
  {
    _detectObject = false;

    if (_highlightedObjects.Count > 0)
    {
      _highlightedObjects.ForEach(p => p.OutlineOff());
      _highlightedObjects.Clear();
    }
  }
}