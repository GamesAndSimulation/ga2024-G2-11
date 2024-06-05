using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
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
    [SerializeField] private GameObject hammer;
    [SerializeField] private Volume postProcessingVolume;
    public float hiddenWeaponY;
    public float showingWeaponY;
    public float coolDownTime = 0.65f;
    [Header("Sound")]
    [SerializeField] private AudioClip weaponEquip;
    [SerializeField] private AudioClip heartbeat;
    
    private GameObject currentWeaponGameObject;
    private float coolDownTimer;
    
    //[SerializeField] private GameObject hammer;
    
    public Weapon HeldWeapon {get; private set;} = Weapon.Revolver;
    private GameObject uiGameObjectBeingHovered;

    private int _uiLayer;
    public static float _initSensitivity;
    private bool _afterOpeningWeaponWheel;
    private bool _inSlowMotion;
    
    void Start()
    {
        _inputManager = InputManager.Instance;
        _uiLayer = LayerMask.NameToLayer("UI");
        _initSensitivity = _virtualCamera.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_MaxSpeed;
        _startTimeScale = Time.timeScale;
        _startFixedDeltaTime = Time.fixedDeltaTime;
        currentWeaponGameObject = hammer;
    }

    void Update()
    {
        if (coolDownTimer > 0 && !_inSlowMotion)
        {
            coolDownTimer -= Time.deltaTime;
            return;
        }
        var pov = _virtualCamera.GetCinemachineComponent<CinemachinePOV>();
        if (_inputManager.IsWeaponWheelOut() && !GameManager.Instance.inFreeCamMode)
        {
            if (GameManager.Instance.inPuzzleMode)
                return;
            
            _afterOpeningWeaponWheel = true;
            
            
            weaponWheel.SetActive(true);
            
            StartSlowMotion();

            coolDownTimer = coolDownTime;
            
            CheckSelectedWeapon(); // Check every frame to update the outline
            
            // Lock player view
            pov.m_HorizontalAxis.m_MaxSpeed = 0;
            pov.m_VerticalAxis.m_MaxSpeed = 0;
            
            // Unlock mouse
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            ToggleVignette();
        }
        else
        {
            if (!_afterOpeningWeaponWheel) return;
            
            StopSlowMotion();
            
            //Update new weapon
            HeldWeapon = CheckSelectedWeapon();
            ChangeWeapon(HeldWeapon);
            
            Debug.Log(HeldWeapon);
            
            weaponWheel.SetActive(false);
            
            pov.m_HorizontalAxis.m_MaxSpeed = _initSensitivity;
            pov.m_VerticalAxis.m_MaxSpeed = _initSensitivity;
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            _afterOpeningWeaponWheel = false;
            
            ToggleVignette();
        }
    }
    
    private void ToggleVignette()
    {
        if (_afterOpeningWeaponWheel)
        {
            DOTween.To(() => postProcessingVolume.weight, x => postProcessingVolume.weight = x, 1f, 0.2f);
        }
        else
        {
            DOTween.To(() => postProcessingVolume.weight, x => postProcessingVolume.weight = x, 0f, 0.2f);
        }
    }

    [SerializeField] private float swordShowHeightDiff = 0.2f;
    [SerializeField] private float hammerShowHeightDiff = 0.2f;

    private void ChangeWeapon(Weapon newWeapon)
    {

        currentWeaponGameObject.transform.DOLocalMoveY(hiddenWeaponY, 0.2f);
        AudioManager.Instance.PlaySound(weaponEquip, false, 5f, true);
        switch (newWeapon)
        {
            case Weapon.Revolver:
                revolver.transform.DOLocalMoveY(showingWeaponY, 0.2f);
                currentWeaponGameObject = revolver;
                revolver.GetComponent<Revolver>().enabled = true;
                hammer.transform.GetChild(0).GetComponent<Hammer>().enabled = false;
                sword.transform.GetChild(0).GetComponent<Sword>().enabled = false;
                break;
            case Weapon.Sword:
                sword.transform.DOLocalMoveY(showingWeaponY + swordShowHeightDiff, 0.2f);
                currentWeaponGameObject = sword;
                revolver.GetComponent<Revolver>().enabled = false;
                hammer.transform.GetChild(0).GetComponent<Hammer>().enabled = false;
                sword.transform.GetChild(0).GetComponent<Sword>().enabled = true;
                break;
            case Weapon.Hammer:
                hammer.transform.DOLocalMoveY(showingWeaponY + hammerShowHeightDiff, 0.2f);
                currentWeaponGameObject = hammer;
                revolver.GetComponent<Revolver>().enabled = false;
                hammer.transform.GetChild(0).GetComponent<Hammer>().enabled = true;
                sword.transform.GetChild(0).GetComponent<Sword>().enabled = false;
                break;
            default:
                break;
        }
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
                if(uiGameObjectBeingHovered != null && uiGameObjectBeingHovered.GetComponent<Outline>() != null)
                    uiGameObjectBeingHovered.GetComponent<Outline>().enabled = false;
                uiGameObjectBeingHovered = curGameObject;
                Outline outline = uiGameObjectBeingHovered.GetComponent<Outline>();
                if (outline == null)
                {
                    var parent = uiGameObjectBeingHovered.transform.parent;
                    Debug.Log("This is the partent of the object: " + parent.name);
                    outline = parent.GetComponent<Outline>();
                    uiGameObjectBeingHovered = parent.gameObject;
                }
                
                if(outline != null)
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
        if (_inSlowMotion)
            return;
        _inSlowMotion = true;
        AudioManager.Instance.PlaySoundLooping(heartbeat, 0.7f, true);
        AudioManager.Instance.StartSlowMo(SlowMotionTimeScale);
        Time.timeScale = SlowMotionTimeScale;
        Time.fixedDeltaTime = _startFixedDeltaTime * SlowMotionTimeScale;
    }
    
    private void StopSlowMotion()
    {
        if(!_inSlowMotion)
            return;
        _inSlowMotion = false;
        AudioManager.Instance.StopSlowMo(SlowMotionTimeScale);
        AudioManager.Instance.StopSoundLooping();
        Time.timeScale = _startTimeScale;
        Time.fixedDeltaTime = _startFixedDeltaTime;
    }
}
