using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
    private GameObject _uiGameObjectBeingHovered;

    private int _uiLayer;
    private float _initSensitivity;
    private bool _afterOpeningWeaponWheel;
    
    void Start()
    {
        _inputManager = InputManager.Instance;
        _uiLayer = LayerMask.NameToLayer("UI");
        _initSensitivity = _virtualCamera.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_MaxSpeed;
        _startTimeScale = Time.timeScale;
        _startFixedDeltaTime = Time.fixedDeltaTime;
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            float y = revolver.transform.position.y;
            revolver.transform.DOMoveY(y - 1.207f, 2f);
        }
        
        var pov = _virtualCamera.GetCinemachineComponent<CinemachinePOV>();
        if (_inputManager.IsWeaponWheelOut())
        {
            weaponWheel.SetActive(true);
            StartSlowMotion();
            CheckSelectedWeapon();
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
            HeldWeapon = CheckSelectedWeapon();
            ChangeWeapon(HeldWeapon);
            Debug.Log(HeldWeapon);
            weaponWheel.SetActive(false);
            pov.m_HorizontalAxis.m_MaxSpeed = _initSensitivity;
            pov.m_VerticalAxis.m_MaxSpeed = _initSensitivity;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            _afterOpeningWeaponWheel = false;
        }
    }
    
    private void ChangeWeapon(Weapon newWeapon)
    {
        if (revolver.activeSelf)
        {
            float currentRevolverY = revolver.transform.position.y;
            revolver.transform.DOMoveY(currentRevolverY - 1.207f, 1f);
            float currentSwordY = sword.transform.position.y;
            sword.transform.DOMoveY(currentSwordY + 1.207f, 1f);
        }
        else
        {
            float currentSwordY = sword.transform.position.y;
            sword.transform.DOMoveY(currentSwordY - 1.207f, 1f);
            float currentRevolverY = revolver.transform.position.y;
            revolver.transform.DOMoveY(currentRevolverY + 1.207f, 1f);
        }
        //revolver.SetActive(false);
        //sword.SetActive(false);
        ////hammer.SetActive(false);
        //switch (newWeapon)
        //{
        //    case Weapon.Revolver:
        //        revolver.SetActive(true);
        //        break;
        //    case Weapon.Sword:
        //        sword.SetActive(true);
        //        break;
        //    case Weapon.Hammer:
        //        //hammer.SetActive(true);
        //        break;
        //    default:
        //        break;
        //}
    }
    
    public Weapon CheckSelectedWeapon()
    {
        return PointerOverUIElement(GetEventSystemRaycastResults());
    }
    
 
    private Weapon PointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
    {
        for (int index = 0; index < eventSystemRaysastResults.Count; )
        {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            var curGameObject = curRaysastResult.gameObject;
            if (curGameObject.layer == _uiLayer)
            {
                if(_uiGameObjectBeingHovered != null)
                    _uiGameObjectBeingHovered.GetComponent<Outline>().enabled = false;
                _uiGameObjectBeingHovered = curGameObject;
                Outline outline = _uiGameObjectBeingHovered.GetComponent<Outline>();
                if (outline == null)
                {
                    var parent = _uiGameObjectBeingHovered.transform.parent;
                    outline = parent.GetComponent<Outline>();
                    _uiGameObjectBeingHovered = parent.gameObject;
                }
                
                outline.enabled = true;
                
                switch (curGameObject.tag)
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
