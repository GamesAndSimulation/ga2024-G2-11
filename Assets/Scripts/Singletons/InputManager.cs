using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private static InputManager _instance;
    
    public static InputManager Instance
    {
        get
        {
            return _instance;
        }
    }
    
    private PlayerControls playerControls;
    private float _initSensitivity;
    private CinemachineVirtualCamera _virtualCamera;
    
    private void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
        playerControls = new PlayerControls();
        _virtualCamera = GameObject.FindWithTag("MainVirtualCamera").GetComponent<CinemachineVirtualCamera>();
        _initSensitivity = _virtualCamera.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_MaxSpeed;
    }
    
    private void OnEnable()
    {
        playerControls.Enable();
    }
    
    private void OnDisable()
    {
        playerControls.Disable();
    }
    
    public void SetLookLock(bool value)
    {
        var povComponent = _virtualCamera.GetCinemachineComponent<CinemachinePOV>();
        povComponent.m_HorizontalAxis.m_MaxSpeed = value ? 0 : _initSensitivity;
        povComponent.m_VerticalAxis.m_MaxSpeed = value ? 0 : _initSensitivity;
    }
    
    public Vector2 GetPlayerMovement()
    {
        return playerControls.PlayerWalk.Movement.ReadValue<Vector2>();
    }
    
    public Vector2 GetMouseDelta()
    {
        return playerControls.PlayerWalk.Look.ReadValue<Vector2>();
    }
    
    public bool PlayerJumpedNow()
    {
        return playerControls.PlayerWalk.Jump.triggered;
    }
    
    public bool IsWeaponWheelOut()
    {
        return playerControls.PlayerWalk.WeaponWheel.IsPressed();
    }
    
    public bool PlayerShotRevolver()
    {
        return playerControls.Revolver.Shoot.triggered;
    }

    public bool PlayerJustReloaded()
    {
        return playerControls.Revolver.Reload.triggered;
    }
    
    // Driving Board
    public bool DrivingForward()
    {
        return playerControls.BoardDriving.Forward.IsPressed();
    }
    
    public bool DrivingBackward()
    {
        return playerControls.BoardDriving.Back.IsPressed();
    }
    
    public bool DrivingLeft()
    {
        return playerControls.BoardDriving.Left.IsPressed();
    }
    
    public bool DrivingRight()
    {
        return playerControls.BoardDriving.Right.IsPressed();
    }

}
