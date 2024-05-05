using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class WeaponWheel : MonoBehaviour
{
    private InputManager _inputManager;
    [SerializeField] private GameObject weaponWheel;
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;
    
    void Start()
    {
        _inputManager = InputManager.Instance;
    }

    void Update()
    {
        var pov = _virtualCamera.GetCinemachineComponent<CinemachinePOV>();
        if (_inputManager.IsWeaponWheelOut())
        {
            weaponWheel.SetActive(true);
            pov.m_HorizontalAxis.m_MaxSpeed = 0;
            pov.m_VerticalAxis.m_MaxSpeed = 0;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            weaponWheel.SetActive(false);
            pov.m_HorizontalAxis.m_MaxSpeed = 0.2f;
            pov.m_VerticalAxis.m_MaxSpeed = 0.2f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
