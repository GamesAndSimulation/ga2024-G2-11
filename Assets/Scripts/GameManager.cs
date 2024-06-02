using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton class
    public static GameManager Instance { get; private set; }

    public bool inPuzzleMode = false;
    public bool inDrivingMode = false;
    public bool inFreeCamMode = false;
    [SerializeField] private GameObject crosshair;
    [SerializeField] private GameObject weaponHolder;
    [SerializeField] private PlayerScript playerScript;
    [SerializeField] private GameObject FPSCounter;
    private float _startTimeScale;
    private float _startFixedDeltaTime;


    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    
    private void Start()
    {
        _startTimeScale = Time.timeScale;
        _startFixedDeltaTime = Time.fixedDeltaTime;
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            FPSCounter.SetActive(!FPSCounter.activeSelf);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            Time.timeScale =(Time.timeScale == 0) ? _startTimeScale : 0;
            Time.fixedDeltaTime = (Time.timeScale == 0) ? _startTimeScale : 0; 
        }
    }

    public void SetShowWalkCrosshairAndGuns(bool show)
    {
        crosshair.SetActive(show);
        weaponHolder.SetActive(show);
    }
    
    public void SetDrivingMode(bool value)
    {
        inDrivingMode = value;
        SetShowWalkCrosshairAndGuns(!value);
        playerScript.enabled = !value;
        
    }
    
    public void SetPuzzleMode(bool value)
    {
        inPuzzleMode = value;
        SetShowWalkCrosshairAndGuns(!value);
    }
    
    
    public Vector3 GetCameraForward()
    {
        var pov = GameObject.FindWithTag("MainVirtualCamera").GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachinePOV>();
        float pitch = pov.m_VerticalAxis.Value;
        float yaw = pov.m_HorizontalAxis.Value;
        
        float pitchRad = pitch * Mathf.Deg2Rad;
        float yawRad = yaw * Mathf.Deg2Rad;
        
        float x = MathF.Cos(pitchRad) * Mathf.Sin(yawRad);
        float y = -MathF.Sin(pitchRad);
        float z = MathF.Cos(pitchRad) * Mathf.Cos(yawRad);
        return new Vector3(x, y, z);
    }


}
