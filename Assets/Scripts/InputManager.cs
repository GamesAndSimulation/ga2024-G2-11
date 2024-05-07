using System.Collections;
using System.Collections.Generic;
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
    }
    
    private void OnEnable()
    {
        playerControls.Enable();
    }
    
    private void OnDisable()
    {
        playerControls.Disable();
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

}
