using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class Revolver : MonoBehaviour
{
    private InputManager inputManager;
    private CinemachineVirtualCamera cinemachineVirtualCamera;
    
    public int Bullets { private set; get; }
    [SerializeField] private int maxBullets;
    
    [SerializeField] private float reloadTime;
    [SerializeField] private float fireRate;
    [SerializeField] private float shootScreenShakeAmplitude;
    [SerializeField] private GameObject MuzzleFlash;
    
    
    void Start()
    {
        inputManager = InputManager.Instance;
        cinemachineVirtualCamera = GameObject.FindWithTag("MainVirtualCamera").GetComponent<CinemachineVirtualCamera>();
        Bullets = maxBullets;
    }

    void Update()
    {

        if (inputManager.PlayerJustReloaded())
        {
            Bullets = maxBullets;
            Debug.Log("Reloaded");
        }

        if (inputManager.PlayerShotRevolver())
        {
            if(Bullets <= 0)
                return;
            
            Bullets--;
            Debug.Log($"{Bullets} bullets left");
            
            // Add muzzle flash
            StartCoroutine(ShowMuzzleFlash(0.1f));
            
            // Add screen shake
            ScreenEffectUtils.Instance.ShakeScreen(0.1f, shootScreenShakeAmplitude);
            
            // Play gun animation
            
            // Raycast and possible bullet trail
        }
    }
    
    private IEnumerator ShowMuzzleFlash(float duration)
    {
        MuzzleFlash.SetActive(true);
        yield return new WaitForSeconds(duration);
        MuzzleFlash.SetActive(false);
    }
    

}
