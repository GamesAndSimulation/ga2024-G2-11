using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    
    public LayerMask groundLayer;
    public float BoardHeight = 2f;
    public float HoverForce;
    public float ForwardForce;
    public float BackwardForce;
    public float TurnForce;
    
    private GameObject[] _topSpheres;
    private GameObject[] _bottomSpheres;
    private Rigidbody _rb;
    private InputManager _inputManager;
    
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
        _rb = GetComponent<Rigidbody>();
        _inputManager = InputManager.Instance;
        var sphereParent = GameObject.Find("TopSpheres");
        _topSpheres = new GameObject[sphereParent.transform.childCount];
        for (int i = 0; i < sphereParent.transform.childCount; i++)
        {
            _topSpheres[i] = sphereParent.transform.GetChild(i).gameObject;
        }
        
        sphereParent = GameObject.Find("BottomSpheres");
        _bottomSpheres = new GameObject[sphereParent.transform.childCount];
        for (int i = 0; i < sphereParent.transform.childCount; i++)
        {
            _bottomSpheres[i] = sphereParent.transform.GetChild(i).gameObject;
        }
    }

    void FixedUpdate()
    {
        HoverBoard();
        HandleInputs();
    }

    private void HandleInputs()
    {
        if (_inputManager.DrivingForward())
        {
            _rb.AddForce(ForwardForce * transform.forward);
        }

        if (_inputManager.DrivingBackward())
        {
            _rb.AddForce(BackwardForce * -transform.forward);
        }

        if (_inputManager.DrivingLeft())
        {
            _rb.AddTorque(TurnForce * -Vector3.up);
        }

        if (_inputManager.DrivingRight())
        {
            _rb.AddTorque(TurnForce * Vector3.up);
        }
    }

    private void HoverBoard()
    {
        int i = 0;
        foreach (var sphere in _topSpheres)
        {
            Ray floorRay = new Ray(sphere.transform.position, Vector3.down);
            if(Physics.Raycast(floorRay, out var hit, 100f, groundLayer))
            {
                Debug.DrawRay(sphere.transform.position, Vector3.down * hit.distance, Color.red);
                _bottomSpheres[i].transform.position = hit.point;
                float distance = BoardHeight - hit.distance;
                float proportionalForce = distance / BoardHeight  * HoverForce;
                proportionalForce = Mathf.Clamp(proportionalForce, 0, HoverForce * 5f);
                if (distance > 0)
                {
                    _rb.AddForceAtPosition(proportionalForce * Vector3.up, sphere.transform.position);
                }
                else
                {
                    _rb.AddForceAtPosition(proportionalForce * Vector3.down, sphere.transform.position);
                }

                
            }
            i++;
        }
    }
}
