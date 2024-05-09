using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private GameObject PuzzleSlotPrefab;
    [SerializeField] private Transform TopLeftCorner;
    private int _placedPieces = 0;

    private Dictionary<Transform, bool> _slotOccupied;
    
    // Each tuples contains the pieces and their respective initial transforms
    private List<Tuple<Vector3, Quaternion>> _piecesTransforms;
    
    private float startingY;
    
    private void Start()
    {
        _piecesTransforms = new List<Tuple<Vector3, Quaternion>>();
        fillPieces();
        populateSlotGrid(6, 6, TopLeftCorner);
        _slotOccupied = new Dictionary<Transform, bool>();
        foreach (Transform slot in PuzzleSlots)
        {
            _slotOccupied.Add(slot, false);
        }
        floatingPosZ = PuzzleFrame.transform.position.z + 0.65f;
        startingPosZ = PuzzleFrame.transform.position.z + 0.45f;
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

    void Update()
    {
        if (!GameManager.instance.inPuzzleMode) return;

        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("Reseting pieces...");
            int i = 0;
            foreach (Transform piece in PuzzleSlots.parent)
            {
                if (piece.CompareTag("PuzzlePiece"))
                {
                   Debug.Log(piece);
                   piece.localPosition = _piecesTransforms[i].Item1;
                   piece.localRotation = _piecesTransforms[i].Item2;
                   i++;
                }
            }

            foreach (var key in _slotOccupied.Keys.ToList())
            {
                _slotOccupied[key] = false;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (_selectedObject == null)
            {
                RaycastHit hit = CastRay();
                if (hit.collider != null && hit.collider.CompareTag("PuzzlePiece"))
                {
                    startingY = hit.collider.transform.position.y;
                    //startingPosZ = hit.collider.transform.position.z;
                    _selectedObject = hit.collider.gameObject;
                    foreach(Transform child in _selectedObject.transform.parent)
                    {
                        Transform slot = FindNearestGridPosition(child.position, out bool isOccupied);
                        Debug.Log($"slot is null? {slot == null}");
                        if (slot != null)
                        {
                            _slotOccupied[slot] = !isOccupied;
                        }
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

        //RaycastHit hit;
        foreach (Transform child in _selectedObject.transform.parent)
        {
            nearestGrid = FindNearestGridPosition(child.position, out isOccupied);
            if (nearestGrid != null && !isOccupied)
            {
                positions.SetValue(nearestGrid, child.GetSiblingIndex());
            }
            else
            {
                return false;
            }
        }
        
        // For every cell of the puzzle piece ======================
        //foreach(Transform child in _selectedObject.transform.parent)
        //{
        //    nearestGrid= FindNearestGridPosition(child.position, out isOccupied);
        //    if (isOccupied)
        //    {
        //        Debug.Log("Can't place piece here.");
        //        return false;
        //    }
        //    positions.SetValue(nearestGrid, child.GetSiblingIndex());
        //}
        //
        // =========================================================
        
        //Debug.Log(positions);

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
        RaycastHit hit;
        Physics.Raycast(currentPos, Vector3.forward * -1f, out hit);
        Debug.Log($"tag: {hit.collider.tag}");
        if (hit.collider.CompareTag("Puzzle"))
        {
            isOccupied = _slotOccupied[hit.transform];
            return hit.transform;
        }

        isOccupied = true;
        return null;
    }

    private void MoveSelectedObject()
    {
        Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y,
        Camera.main.WorldToScreenPoint(_selectedObject.transform.position).z);
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(position);
        _selectedObject.transform.parent.position = new Vector3(worldPosition.x, worldPosition.y, floatingPosZ);
        foreach (Transform child in _selectedObject.transform.parent)
        {
            Debug.DrawRay(child.position, child.forward * -1f, Color.red);
        }
        
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

    private void populateSlotGrid(int sizeX, int sizeY, Transform topLeftCorner)
    {
        Quaternion rotation = topLeftCorner.localRotation;
        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                GameObject slot = Instantiate(PuzzleSlotPrefab, new Vector3(topLeftCorner.localPosition.x + i, topLeftCorner.localPosition.y - j, topLeftCorner.localPosition.z), rotation);
                slot.transform.parent = PuzzleSlots;
                float xOffset = i * slot.transform.localScale.x;
                float yOffset = j * slot.transform.localScale.y;
                slot.transform.localPosition = new Vector3(topLeftCorner.localPosition.x - xOffset, topLeftCorner.localPosition.y - yOffset, topLeftCorner.localPosition.z);
            }
        }
    }
}
