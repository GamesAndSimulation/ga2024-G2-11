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
    private float _startTimeScale;
    private float _startFixedDeltaTime;
    public float SlowMotionTimeScale;
    
    [SerializeField] private GameObject weaponWheel;
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;
    [SerializeField] private GameObject sword;
    [SerializeField] private GameObject revolver;
    //[SerializeField] private GameObject hammer;
    
    public Weapon HeldWeapon {get; private set;} = Weapon.Revolver;

    private int UILayer;
    private bool _afterOpeningWeaponWheel = false;
    
    void Start()
    {
        _inputManager = InputManager.Instance;
        UILayer = LayerMask.NameToLayer("UI");
        _startTimeScale = Time.timeScale;
        _startFixedDeltaTime = Time.fixedDeltaTime;
    }

    void Update()
    {
        var pov = _virtualCamera.GetCinemachineComponent<CinemachinePOV>();
        if (_inputManager.IsWeaponWheelOut())
        {
            weaponWheel.SetActive(true);
            StartSlowMotion();
            pov.m_HorizontalAxis.m_MaxSpeed = 0;
            pov.m_VerticalAxis.m_MaxSpeed = 0;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            _afterOpeningWeaponWheel = true;
        }
        else
        {
            if (!_afterOpeningWeaponWheel) return;
            StopSlowMotion();
            HeldWeapon = GetSelectedWeapon();
            ChangeWeapon(HeldWeapon);
            Debug.Log(HeldWeapon);
            weaponWheel.SetActive(false);
            pov.m_HorizontalAxis.m_MaxSpeed = 0.2f;
            pov.m_VerticalAxis.m_MaxSpeed = 0.2f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            _afterOpeningWeaponWheel = false;
        }
    }
    
    private void ChangeWeapon(Weapon newWeapon)
    {
        revolver.SetActive(false);
        sword.SetActive(false);
        //hammer.SetActive(false);
        switch (newWeapon)
        {
            case Weapon.Revolver:
                revolver.SetActive(true);
                break;
            case Weapon.Sword:
                sword.SetActive(true);
                break;
            case Weapon.Hammer:
                //hammer.SetActive(true);
                break;
            default:
                break;
        }
    }
    
    public Weapon GetSelectedWeapon()
    {
        return PointerOverUIElement(GetEventSystemRaycastResults());
    }
 
 
    private Weapon PointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
    {
        for (int index = 0; index < eventSystemRaysastResults.Count; index++)
        {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            if (curRaysastResult.gameObject.layer == UILayer)
                switch (curRaysastResult.gameObject.tag)
                {
                    case "RevolverUI":
                        return Weapon.Revolver;
                    case "SwordUI":
                        return Weapon.Sword;
                    case "HammerUI":
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
    
    private void StartSlowMotion()
    {
        Time.timeScale = SlowMotionTimeScale;
        Time.fixedDeltaTime = _startFixedDeltaTime * SlowMotionTimeScale;
    }
    
    private void StopSlowMotion()
    {
        Time.timeScale = _startTimeScale;
        Time.fixedDeltaTime = _startFixedDeltaTime;
    }
}
