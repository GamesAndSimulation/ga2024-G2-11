using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    
    public LayerMask groundLayer;
    public float BoardHeight = 2f;
    public float HoverForce = 10f;
    
    private GameObject[] _topSpheres;
    private GameObject[] _bottomSpheres;
    private Rigidbody _rb;
    
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
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
        DrawDebugRays();
    }

    private void DrawDebugRays()
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
                // Clamp force
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
