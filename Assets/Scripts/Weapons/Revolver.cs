using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class Revolver : MonoBehaviour
{
    private InputManager inputManager;
    private Animator _revolverAnimator;
    
    private bool _isReloading;
    private float _shootTimer;
    
    public int Bullets { private set; get; }
    
    [SerializeField] private int maxBullets;
    [SerializeField] private float reloadTime;
    [SerializeField] private float fireRateDelay;
    [SerializeField] private float shootScreenShakeAmplitude;
    [SerializeField] private GameObject MuzzleFlash;

    [SerializeField] private TextMeshProUGUI ammoText;
    
    
    void Start()
    {
        _shootTimer = 0;
        inputManager = InputManager.Instance;
        Bullets = maxBullets;
        ammoText.text = Bullets.ToString();
        _revolverAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        
        if(GameManager.Instance.inPuzzleMode)
            return;
        
        // Reload
        if (inputManager.PlayerJustReloaded() && Bullets < maxBullets && !_isReloading)
        {
            StartCoroutine(Reload());
        }

        // Shoot
        if (inputManager.PlayerShotRevolver() && !_isReloading && Bullets > 0 && _shootTimer <= 0)
        {
            _shootTimer = fireRateDelay;
            Bullets--;
            ammoText.text = Bullets.ToString();
            Debug.Log($"{Bullets} bullets left");
            
            _revolverAnimator.SetTrigger("Shoot");
            
            // Add muzzle flash
            StartCoroutine(ShowMuzzleFlash(0.1f));
            
            // Add screen shake
            ScreenEffectUtils.Instance.ShakeScreen(0.1f, shootScreenShakeAmplitude);
            
            // Play gun animation
            
            // Raycast and possible bullet trail
        }
        
        if(_shootTimer > 0)
            _shootTimer -= Time.deltaTime;
    }

    private IEnumerator Reload()
    {
        _isReloading = true;
        _revolverAnimator.SetTrigger("Reload");
        yield return new WaitWhile(() => _revolverAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Reload");
        //Debug.Log("Animation finished.");
        Bullets = maxBullets;
        ammoText.text = Bullets.ToString();
        _isReloading = false;
    }
    
    
    private IEnumerator ShowMuzzleFlash(float duration)
    {
        MuzzleFlash.SetActive(true);
        yield return new WaitForSeconds(duration);
        MuzzleFlash.SetActive(false);
    }
    

}
