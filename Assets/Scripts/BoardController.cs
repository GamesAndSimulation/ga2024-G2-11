using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    
    public LayerMask groundLayer;
    public GameObject DustParticlesPrefab;
    public Transform Sail;
    public float BoardHeight = 2f;
    public float HoverForce;
    public float ForwardForce;
    public float BackwardForce;
    public float TurnForce;
    public float dustSpawnVelocity;
    
    private GameObject[] _topSpheres;
    private GameObject[] _bottomSpheres;
    private Rigidbody _rb;
    private InputManager _inputManager;
    
    private float spawnDustTimer;
    [SerializeField] private float dustSpawnRate = 0.5f;
    [SerializeField] private Transform dustSpawnPoint;
    
    
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
        spawnDustTimer = dustSpawnRate;
    }

    private void Update()
    {
        if (_inputManager.DrivingForward() && _rb.velocity.magnitude > dustSpawnVelocity)
        {
            spawnDustTimer -= Time.deltaTime;
            if (spawnDustTimer <= 0)
            {
                var particlesObj = Instantiate(DustParticlesPrefab, dustSpawnPoint.position , Quaternion.identity);
                particlesObj.GetComponent<ParticleSystem>().Play();
                Destroy(particlesObj, 5f);
                spawnDustTimer = dustSpawnRate;
            }
        }
        if(Input.GetKeyDown(KeyCode.E)) 
        {
            GameManager.Instance.SetDrivingMode(false);
            GameObject.FindWithTag("MainVirtualCamera").GetComponent<CinemachineVirtualCamera>().Priority = 1;
            var player = GameObject.FindWithTag("Player");
            player.transform.position = transform.position + new Vector3(0, player.transform.localScale.y * 4f, 0);
            GetComponentInChildren<CinemachineFreeLook>().Priority = 0;
            this.enabled = false;
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
            Sail.DOLocalRotate(new Vector3(0, -30f, 0), 1.6f).SetEase(Ease.OutQuad);
        }

        if (_inputManager.DrivingRight())
        {
            _rb.AddTorque(TurnForce * Vector3.up);
            Sail.DOLocalRotate(new Vector3(0, 30f, 0), 1.6f).SetEase(Ease.OutQuad);
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
                else if(distance < 0)
                {
                    _rb.AddForceAtPosition(proportionalForce * Vector3.down, sphere.transform.position);
                }

                
            }
            i++;
        }
    }
}
