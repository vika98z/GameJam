using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PickUp : MonoBehaviour
{
  [SerializeField] private Camera camera;
  [SerializeField] private Transform objectHolder;
  [SerializeField] private PickableObject item;
  [SerializeField] private float pickUpDistance;
  [SerializeField] private GameManager gameManager;

  [Space] public string dropText = "Нажмите ЛКМ, чтобы бросить предмет";
  public string pickUpText = "Нажмите Е, чтобы взять предмет";
  public string connectText = "Нажмите ЛКМ, чтобы объединить предметы";
  public string connectAndRotateText = "Крутите колесико мыши, чтобы вращать предмет. Нажмите ЛКМ, чтобы объединить предметы";
  
  private bool _carryObject;

  public List<PickableObject> _highlightedObjectsList = new List<PickableObject>();
  private bool _detectObject;
  private LayerMask _layerMask;

  private void Awake()
  {
    var dist = PlayerPrefs.GetInt("Distance");
    if (dist > 0)
      pickUpDistance = dist;
    else
    {
      pickUpDistance = 1.5f;
    }
    _layerMask = ~ LayerMask.GetMask("Player");
  }

  private void Update()
  {
    if (gameManager.GameIsPaused)
      return;
    
    var directionRay = camera.ScreenPointToRay(Input.mousePosition);
    
    var distance = pickUpDistance + 
                   Vector3.Distance(transform.position, camera.transform.position);
      
    if (Physics.Raycast(directionRay, out var hit, distance, _layerMask))
    {
      if (hit.collider.CompareTag("PickUp") || hit.collider.CompareTag("RectanglePart"))
      {
        var raycastedObject = hit.collider.gameObject.GetComponent<PickableObject>();

        if (hit.collider.CompareTag("RectanglePart"))
          raycastedObject = hit.collider.gameObject.transform.parent.parent.GetComponent<PickableObject>();
        
        if (_highlightedObjectsList.Count > 1)
          ClearHighlighted();
        
        if (raycastedObject != null && raycastedObject != item && raycastedObject.IsHighlightable)
        {
          CheckObjectAndSetHighlight(raycastedObject);

          
          if (CanConnect())
          {
            if (item.TypeOfDetail == DetailType.Cube)
              gameManager.SetHelpText(connectText);
            else
              gameManager.SetHelpText(connectAndRotateText);
          }
          else if (CanPickUp())
            gameManager.SetHelpText(pickUpText);
          else
          {
            ClearHighlighted();
          }
        }
      }
      else
        ClearHighlighted();
    }
    else
      ClearHighlighted();

    if (CanPickUp() && Input.GetKeyDown(KeyCode.E))
      PickUpObject();
    else if (CanConnect() && Input.GetMouseButton(0))
    {
      if (item.Connect(_highlightedObjectsList.Last()))
        Throw();
      return;
    }

    if (Input.GetMouseButton(0))
      Throw();

    bool CanPickUp() =>
       _detectObject && !_carryObject && !_highlightedObjectsList.Last().IsConnected && _highlightedObjectsList.Last().IsHighlightable;

    bool CanConnect() =>
      item != null && _highlightedObjectsList.Count > 0 && item.IsConnectable
      && _detectObject && _carryObject 
      && item != _highlightedObjectsList.Last() && _highlightedObjectsList.Last().IsHighlightable && _highlightedObjectsList.Last().IsConnected;
  }

  private void CheckObjectAndSetHighlight(PickableObject raycastedObject)
  {
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
      
      gameManager.SetHelpText("");
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
    if (_carryObject)
      gameManager.SetHelpText(dropText);
    else 
      gameManager.SetHelpText("");
    
    _detectObject = false;

    if (_highlightedObjectsList.Count > 0)
      _highlightedObjectsList.ForEach(p => p?.OutlineOff());

    _highlightedObjectsList.Clear();
    _highlightedObjectsList = new List<PickableObject>();
  }
}