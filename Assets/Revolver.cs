using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Revolver : MonoBehaviour
{
    private InputManager inputManager;
    
    public int bullets { private set; get; }
    [SerializeField] private int maxBullets;
    
    [SerializeField] private float reloadTime;
    [SerializeField] private float fireRate;
    
    void Start()
    {
        inputManager = InputManager.Instance;
        bullets = maxBullets;
    }

    void Update()
    {

        if (inputManager.PlayerJustReloaded())
        {
            bullets = maxBullets;
            Debug.Log("Reloaded");
        }

        if (inputManager.PlayerShotRevolver())
        {
            if(bullets > 0)
                bullets--;
            Debug.Log($"{bullets} bullets left");
            
            // Add muzzle flash
            
            // Add screen shake
            
            // Raycast and possible bullet trail
        }
    }
}
