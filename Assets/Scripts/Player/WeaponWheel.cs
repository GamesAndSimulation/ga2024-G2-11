using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;

public class WeaponWheel : MonoBehaviour
{
    public enum Weapon
    {
        Revolver,
        Sword,
        Hammer,
        None
    }
    
    private InputManager _inputManager;
    [SerializeField] private GameObject weaponWheel;
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;
    
    public Weapon HeldWeapon {get; private set;} = Weapon.Revolver;

    private int UILayer;
    private bool _afterOpeningWeaponWheel = false;
    
    void Start()
    {
        _inputManager = InputManager.Instance;
        UILayer = LayerMask.NameToLayer("UI");
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
            _afterOpeningWeaponWheel = true;
        }
        else
        {
            if (!_afterOpeningWeaponWheel) return;
            HeldWeapon = GetSelectedWeapon();
            Debug.Log(HeldWeapon);
            weaponWheel.SetActive(false);
            pov.m_HorizontalAxis.m_MaxSpeed = 0.2f;
            pov.m_VerticalAxis.m_MaxSpeed = 0.2f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            _afterOpeningWeaponWheel = false;
        }
    }
    
    //Returns 'true' if we touched or hovering on Unity UI element.
    public Weapon GetSelectedWeapon()
    {
        return PointerOverUIElement(GetEventSystemRaycastResults());
    }
 
 
    //Returns 'true' if we touched or hovering on Unity UI element.
    private Weapon PointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
    {
        for (int index = 0; index < eventSystemRaysastResults.Count; index++)
        {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            if (curRaysastResult.gameObject.layer == UILayer)
                switch (curRaysastResult.gameObject.name)
                {
                    case "Revolver":
                        return Weapon.Revolver;
                    case "Sword":
                        return Weapon.Sword;
                    case "Hammer":
                        return Weapon.Hammer;
                    default :
                        return Weapon.None;
                }
        }
        return Weapon.None;
    }
 
 
    //Gets all event system raycast results of current mouse or touch position.
    static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    } 
}
