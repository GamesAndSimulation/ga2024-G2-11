using System;using System.Collections;
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
    public Transform _turretHead;
    private bool _shooting = false;
    [SerializeField] private float dieForce = 3;

    [SerializeField]
    private AudioClip explosionSound;
    public bool dead = false;
    
    
    public void TakeDamage(float damage)
    {
        Debug.Log("Turret took damage");
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
        _turretHead = transform.Find("TurretHead");
    }

    private void Die()
    {
        AudioManager.Instance.PlaySound(explosionSound, true, 0.3f);
        dead = true;
        GetComponent<EnemyFov>().enabled = false;
        var explosion = Instantiate(Resources.Load("Prefabs/SwordHitParticles"), transform.position, Quaternion.identity) as GameObject;
        explosion.transform.localScale *= 9;
        GetComponentInChildren<Animator>().SetTrigger("TurretDie");
        GetComponentInChildren<Light>().enabled = false;
        var rb = transform.AddComponent<Rigidbody>();
        rb.AddForce(new Vector3(UnityEngine.Random.Range(-1f, 1f), 1, UnityEngine.Random.Range(-1f, 1f)) * dieForce, ForceMode.Impulse);
        SetCanShoot(false);
        enabled = false;
    }
    
    private IEnumerator ShootRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(shootInterval);
        while (true)
        {
            if(dead) yield break;
            yield return wait;
            var bullet = Instantiate(Resources.Load("Prefabs/BulletTrail"), _turretHead.position, Quaternion.identity);
            bullet.GetComponent<Bullet>().SetDamage(damagePerBullet);
            Vector3 direction = (GameManager.Instance.GetPlayerPosition() - _turretHead.position).normalized;
            bullet.GetComponent<Rigidbody>().AddForce(direction * bulletSpeed, ForceMode.Impulse);
            // Play shoot sound
        }
    }

    public void SetCanShoot(bool canShoot)
    {
        if (!canShoot)
        {
            _shooting = false;
            StopAllCoroutines();
        }
        else if (!_shooting)
        {
            StartCoroutine(ShootRoutine());
            _shooting = true;
        }
    }

}
