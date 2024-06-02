using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public float health = 100f;
    public float damagePerBullet = 10f;
    public float shootInterval = 1f;
    public float bulletSpeed = 300f;
    [SerializeField] private GameObject _turretHead;
    
    public void TakeDamage(float damage)
    {
        Debug.Log("Turret took damage");
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
    
    private IEnumerator ShootRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(shootInterval);
        while (true)
        {
            yield return wait;
            var bullet = Instantiate(Resources.Load("Prefabs/BulletTrail"), _turretHead.transform.position, Quaternion.identity, transform);
            bullet.GetComponent<Bullet>().SetDamage(damagePerBullet);
            Vector3 direction = (GameManager.Instance.GetPlayerPosition() - _turretHead.transform.position).normalized;
            bullet.GetComponent<Rigidbody>().AddForce(direction * bulletSpeed, ForceMode.Impulse);
            // Play shoot sound
        }
    }

    public void SetCanShoot(bool canShoot)
    {
        if(canShoot)
            StartCoroutine(ShootRoutine());
        else
            StopAllCoroutines();
    }

}
