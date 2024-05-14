using System;using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class SigilPuzzle: MonoBehaviour
{
    [SerializeField] private int slotsWidth = 6;
    [SerializeField] private int slotsHeight = 6;
    [SerializeField] private Transform TopLeftCorner;
    [SerializeField] private float resetTime = 0.38f;
    [SerializeField] private float smoothingSpeed = 0.1f;

    private GameObject _selectedObject;
    private GameObject PuzzleSlotPrefab;
    private Transform PuzzleSlots;
    private int _placedPiecesNum;
    private float floatingPosZ;
    private float startingPosZ;

    // Data Structures
    private Dictionary<Transform, bool> _slotOccupied;  // Tracks whether slots are occupied
    private List<Tuple<Vector3, Quaternion>> _piecesTransforms;  // Stores initial transforms for undo or reset
    
    private void Start()
    {
        _placedPiecesNum = 0;
        PuzzleSlotPrefab = Resources.Load<GameObject>("Prefabs/PuzzleSlot");
        PuzzleSlots = transform.Find("PuzzleArea").Find("Slots");
        _piecesTransforms = new List<Tuple<Vector3, Quaternion>>();
        fillPieces();
        PopulateSlotGrid(slotsWidth, slotsHeight, TopLeftCorner);
        _slotOccupied = new Dictionary<Transform, bool>();
        foreach (Transform slot in PuzzleSlots)
        {
            if(slot.gameObject.activeSelf)
                _slotOccupied.Add(slot, false);
        }
        Debug.Log($"_slotOccupied count: {_slotOccupied.Count}");
        floatingPosZ = transform.position.z + 0.65f;
        startingPosZ = transform.position.z + 0.45f;
        this.enabled = false;
    }

    private void fillPieces()
    {
        foreach (Transform child in PuzzleSlots.parent)
        {
            if (child.CompareTag("PuzzlePiece"))
            {
                _piecesTransforms.Add(new Tuple<Vector3, Quaternion>(child.transform.localPosition, child.transform.localRotation));
            }
        }
    }

    public void ResetPuzzleBoard()
    {
        int i = 0;
        foreach (Transform piece in PuzzleSlots.parent)
        {
            if (piece.CompareTag("PuzzlePiece"))
            {
                piece.DOLocalMove(_piecesTransforms[i].Item1, resetTime);
                piece.DOLocalRotate(_piecesTransforms[i].Item2.eulerAngles, resetTime);
                i++;
            }
        }

        foreach (var key in _slotOccupied.Keys.ToList())
        {
            _slotOccupied[key] = false;
        }
        
        _placedPiecesNum = 0;
    }

    void Update()
    {
        if (!GameManager.Instance.inPuzzleMode) return;

        CastRay();

        // Reset board
        if (Input.GetKeyDown(KeyCode.T))
        {
            ResetPuzzleBoard();
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (_selectedObject == null)
            {
                RaycastHit hit = CastRay();
                if (hit.collider != null && hit.collider.CompareTag("PuzzlePiece"))
                {
                    //startingPosZ = hit.collider.transform.position.z;
                    _selectedObject = hit.collider.gameObject;
                    foreach(Transform child in _selectedObject.transform.parent)
                    {
                        Transform slot = FindNearestGridPosition(child, out bool isOccupied);
                        Debug.Log($"slot is null? {slot == null}");
                        if (slot != null)
                        {
                            _slotOccupied[slot] = false; //!isOccupied;
                        }
                    }
                }
            }
            else
            {
                if (PlaceObjectInGrid()) // Place was successful
                {
                    _selectedObject = null;
                    CheckGridFull();
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

    private void CheckGridFull()
    {
        Debug.Log($"slots count: {slotsWidth * slotsHeight}");
        Debug.Log($"_placedPiecesNum: {_placedPiecesNum}");
        if(_placedPiecesNum >= slotsWidth * slotsHeight) // TODO I have no idea why I have to do it this way
        {
            MeshRenderer frame = GameObject.Find("Frame").GetComponent<MeshRenderer>();
            frame.material.DOColor(Color.green, 0.5f);
            
        }
    }

    // Returns true if successful
    private bool PlaceObjectInGrid()
    {
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y,
        Camera.main.WorldToScreenPoint(_selectedObject.transform.position).z);
        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);
        Transform[] positions = new Transform[_selectedObject.transform.parent.childCount];

        bool someOutside = false;
        bool someInside = false;

        // Checking if every slot is free/available
        foreach (Transform child in _selectedObject.transform.parent)
        {
            var nearestGrid = FindNearestGridPosition(child, out var isOccupied);
            if (nearestGrid != child) // Means it's trying to place a piece in the grid
            {
                if (isOccupied || someOutside)
                {
                    return false;
                }
                someInside = true;
            }
            else
            {
                if (someInside)
                {
                    return false;
                }
                someOutside = true;
            }
            
            
            positions.SetValue(nearestGrid, child.GetSiblingIndex());
        }
        
        // Placing every cell of the piece in each slot, since
        // we now know that every slot is free
        foreach (Transform child in _selectedObject.transform.parent)
        {
            if(someInside)
                _placedPiecesNum++;
            if(someOutside)
                _placedPiecesNum--;
            var position = positions[child.GetSiblingIndex()].position;
            child.position = new Vector3(position.x, position.y, startingPosZ);
            _slotOccupied[positions[child.GetSiblingIndex()]] = true;
        }
        return true;
    }

    private Transform FindNearestGridPosition(Transform currentPos, out bool isOccupied)
    {
        RaycastHit hit;
        Physics.Raycast(currentPos.position, Vector3.forward * -1f, out hit);
        Debug.Log($"tag: {hit.collider.tag}");
        if (hit.collider.CompareTag("Puzzle"))
        {
            isOccupied = _slotOccupied[hit.transform];
            return hit.transform;
        }

        isOccupied = false;
        return currentPos;
    }

    private void MoveSelectedObject()
    {
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y,
            Camera.main.WorldToScreenPoint(_selectedObject.transform.position).z);
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePos);

        Vector3 targetPosition = new Vector3(worldPosition.x, worldPosition.y, floatingPosZ);

        _selectedObject.transform.parent.position = Vector3.Lerp(_selectedObject.transform.parent.position, targetPosition, smoothingSpeed);

        foreach (Transform child in _selectedObject.transform.parent)
        {
            Debug.DrawRay(child.position, child.forward * -1f, Color.red);
        }
    }

    public float offset = 0.5f;

    private RaycastHit CastRay()
    {
        Camera cam = Camera.main;
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 100f;
        mousePos = cam.ScreenToWorldPoint(mousePos);
        
        Vector3 pos = Input.mousePosition;
        pos.z = cam.nearClipPlane + offset;
        
        //Debug.DrawRay(cam.transform.position, mousePos - cam.transform.position, Color.blue);
        
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        Debug.DrawLine ( ray.origin, ray.origin + ray.direction * 100, Color.red );
        Debug.DrawRay ( ray.origin, ray.direction * 100, Color.blue); 
        RaycastHit hit;
        
        if(Physics.Raycast(ray, out hit))
        {
            //Debug.Log(hit.collider.name);
        }

        return hit;

    }

    private float _boardZ;

    private void PopulateSlotGrid(int sizeX, int sizeY, Transform topLeftCorner)
    {
        Quaternion rotation = topLeftCorner.localRotation;
        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                GameObject slot = Instantiate(PuzzleSlotPrefab, new Vector3(topLeftCorner.localPosition.x + i, topLeftCorner.localPosition.y - j, topLeftCorner.localPosition.z), rotation);
                _boardZ = slot.transform.position.z;
                slot.transform.parent = PuzzleSlots;
                float xOffset = i * slot.transform.localScale.x;
                float yOffset = j * slot.transform.localScale.y;
                slot.transform.localPosition = new Vector3(topLeftCorner.localPosition.x - xOffset, topLeftCorner.localPosition.y - yOffset, topLeftCorner.localPosition.z);
            }
        }
    }
}
