using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabber : MonoBehaviour
{
    private GameObject _selectedObject;
    
    private float floatingPosZ = -13.2f;
    private float startingPosZ = -10f;
    
    public Transform PuzzleSlots;
    public GameObject PuzzleFrame;
    public Material WinFrameColor;
    [SerializeField] private float gizmoSize = 0.1f;
    [SerializeField] private Color gizmoColor = Color.red;
    private int _placedPieces = 0;

    private Dictionary<Transform, bool> _slotOccupied;
    
    private float startingY;
    
    private void Start()
    {
        _slotOccupied = new Dictionary<Transform, bool>();
        foreach (Transform slot in PuzzleSlots)
        {
            _slotOccupied.Add(slot, false);
        }
        floatingPosZ = PuzzleFrame.transform.position.z + 0.5f;
        startingPosZ = PuzzleFrame.transform.position.z + 0.3f;
    }

    void Update()
    {
        if (!GameManager.instance.inPuzzleMode) return;

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse Down Puzzle");
            if (_selectedObject == null)
            {
                RaycastHit hit = CastRay();
                if (hit.collider != null && hit.collider.CompareTag("PuzzlePiece"))
                {
                    startingY = hit.collider.transform.position.y;
                    startingPosZ = hit.collider.transform.position.z;
                    _selectedObject = hit.collider.gameObject;
                    foreach(Transform child in _selectedObject.transform.parent)
                    {
                        Transform slot = FindNearestGridPosition(child.position, out bool isOccupied);
                        _slotOccupied[slot] = false;
                    }
                    _placedPieces--; // TODO this is wrong. It shouldn't decrement if we are picking non-placed pieces
                }
            }
            else
            {
                if (PlaceObjectInGrid()) // Place was successful
                {
                    _selectedObject = null;
                    _placedPieces++;
                    if (_placedPieces == 5)
                    {
                        Debug.Log("Puzzle complete!");
                        PuzzleFrame.GetComponent<Renderer>().material = WinFrameColor;
                    }
                    Debug.Log("Piece placed.");
                }
                
            }
        }

        if (_selectedObject != null)
        {
            if (Input.GetMouseButtonDown(1))
            {
                _selectedObject.transform.parent.Rotate(0, 0, 90, Space.World);
            }
            MoveSelectedObject();
        }
    }

    // Returns true if successful
    private bool PlaceObjectInGrid()
    {
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y,
        Camera.main.WorldToScreenPoint(_selectedObject.transform.position).z);
        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);
        bool isOccupied;
        Transform nearestGrid;
        Transform[] positions = new Transform[_selectedObject.transform.parent.childCount];
        foreach(Transform child in _selectedObject.transform.parent)
        {
            nearestGrid= FindNearestGridPosition(child.position, out isOccupied);
            if (isOccupied)
            {
                Debug.Log("Can't place piece here.");
                return false;
            }
            positions.SetValue(nearestGrid, child.GetSiblingIndex());
        }
        
        Debug.Log(positions);

        foreach (Transform child in _selectedObject.transform.parent)
        {
            var position = positions[child.GetSiblingIndex()].position;
            child.position = new Vector3(position.x, position.y, startingPosZ);
            _slotOccupied[positions[child.GetSiblingIndex()]] = true;
        }
        return true;
    }

    private Transform FindNearestGridPosition(Vector3 currentPos, out bool isOccupied)
    {
        float minDistance = float.MaxValue;
        Transform nearestSlot = null;
        foreach (Transform slot in PuzzleSlots)
        {
            float distance = Vector3.Distance(currentPos, slot.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestSlot = slot;
            }
        }
        
        isOccupied = _slotOccupied[nearestSlot];

        return nearestSlot;
    }

    private void MoveSelectedObject()
    {
        Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y,
        Camera.main.WorldToScreenPoint(_selectedObject.transform.position).z);
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(position);
        _selectedObject.transform.parent.position = new Vector3(worldPosition.x, worldPosition.y, floatingPosZ);
    }

    private RaycastHit CastRay()
    {
        Vector3 screenMousePosFar = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.farClipPlane);
        Vector3 screenMousePosNear = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane);
        Vector3 worldMousePosFar = Camera.main.ScreenToWorldPoint(screenMousePosFar);
        Vector3 worldMousePosNear = Camera.main.ScreenToWorldPoint(screenMousePosNear);

        RaycastHit hit;
        Physics.Raycast(worldMousePosNear, worldMousePosFar - worldMousePosNear, out hit);
        return hit;
    } 
}
