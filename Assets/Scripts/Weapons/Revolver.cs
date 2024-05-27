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
    
    public int BulletsInChamber { private set; get; }
    public int StoredBullets;
    
    [SerializeField] private int maxBullets;
    [SerializeField] private float reloadTime;
    [SerializeField] private float fireRateDelay;
    [SerializeField] private float shootScreenShakeAmplitude;
    [SerializeField] private GameObject MuzzleFlash;
    [SerializeField] private float damage;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private Transform _bulletSpawnPoint;

    [SerializeField] private TextMeshProUGUI ammoChamberText;
    [SerializeField] private TextMeshProUGUI ammoStoredText;
    
    
    void Start()
    {
        _shootTimer = 0;
        inputManager = InputManager.Instance;
        BulletsInChamber = Mathf.Clamp(StoredBullets, 0, maxBullets);
        StoredBullets -= BulletsInChamber;
        UpdateAmmoCount();
        _revolverAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        
        if(GameManager.Instance.inPuzzleMode)
            return;
        
        // Reload
        if (inputManager.PlayerJustReloaded() && BulletsInChamber < maxBullets && !_isReloading)
        {
            StartCoroutine(Reload());
        }

        // Shoot
        if (inputManager.PlayerShotRevolver() && !_isReloading && BulletsInChamber > 0 && _shootTimer <= 0)
        {
            _shootTimer = fireRateDelay;
            BulletsInChamber--;
            UpdateAmmoCount();
            Debug.Log($"{BulletsInChamber} bullets left");
            
            _revolverAnimator.SetTrigger("Shoot");
            
            StartCoroutine(ShowMuzzleFlash(0.1f));
            
            // Add screen shake
            ScreenEffectUtils.Instance.ShakeScreen(0.1f, shootScreenShakeAmplitude);
            
            ShootBullet();

        }
        
        if(_shootTimer > 0)
            _shootTimer -= Time.deltaTime;
    }
    
    private void ShootBullet()
    {
        var bullet = Instantiate(Resources.Load<GameObject>("Prefabs/BulletTrail"), _bulletSpawnPoint.position,
            Quaternion.identity);
        bullet.GetComponent<Bullet>().SetDamage(damage);
        Vector3 direction = GameManager.Instance.GetCameraForward();
        RaycastHit hit;
        if (Physics.Raycast(_bulletSpawnPoint.position, direction, out hit, 1000f))
        {
            Debug.Log($"Bullet raycast hit: {hit.transform.name}");
            direction = (hit.point - _bulletSpawnPoint.position).normalized;
        }
        bullet.GetComponent<Rigidbody>().AddForce(direction * bulletSpeed, ForceMode.Impulse);

    }

    private IEnumerator Reload()
    {
        _isReloading = true;
        _revolverAnimator.SetTrigger("Reload");
        yield return new WaitWhile(() => _revolverAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Reload");
        //Debug.Log("Animation finished.");
        
        BulletsInChamber = Mathf.Clamp(StoredBullets, 0, maxBullets);
        StoredBullets -= BulletsInChamber;
        UpdateAmmoCount();
        
        _isReloading = false;
    }
    
    
    private IEnumerator ShowMuzzleFlash(float duration)
    {
        MuzzleFlash.SetActive(true);
        yield return new WaitForSeconds(duration);
        MuzzleFlash.SetActive(false);
    }

    private void UpdateAmmoCount()
    {
        ammoChamberText.text = BulletsInChamber.ToString();
        ammoStoredText.text = StoredBullets.ToString();
    }
    

}
