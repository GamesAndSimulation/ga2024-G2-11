using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float _bulletDamage;
    private PlayerStats _playerScript;
    private Rigidbody _rb;
    private Vector3 _storedVelocity;
    private bool pausedLastFrame;
    
    private void Start()
    {
        _playerScript = GameObject.FindWithTag("Player").GetComponent<PlayerStats>();
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (GameManager.Instance.gamePaused)
        {
            _rb.isKinematic = true;
            _storedVelocity = _rb.velocity;
            _rb.velocity = Vector3.zero;

        }
        else if(pausedLastFrame)
        {
            _rb.velocity = _storedVelocity;
            pausedLastFrame = false;
        }
    }
    
    private void OnCollisionEnter(Collision other)
    {
        
        if (other.transform.CompareTag("Enemy"))
        {
            other.transform.GetComponent<Enemy>().TakeDamage(_bulletDamage);
            var particles = Instantiate(Resources.Load<GameObject>("Prefabs/SwordHitParticles"), other.GetContact(0).point, Quaternion.identity);
            Destroy(particles, 1f);
        }

        else if (other.transform.CompareTag("Turret"))
        {
            other.transform.GetComponent<Turret>().TakeDamage(_bulletDamage);
            var particles = Instantiate(Resources.Load<GameObject>("Prefabs/SwordHitParticles"), other.GetContact(0).point, Quaternion.identity);
            Destroy(particles, 1f);
        }
        else if (other.transform.CompareTag("Player"))
        {
            _playerScript.TakeDamage(_bulletDamage);
        }

        transform.GetComponent<Rigidbody>().isKinematic = true;
        //transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
        transform.GetComponent<SphereCollider>().enabled = false;
        Destroy(gameObject, 2f);
    }

    public void SetDamage(float damage)
    {
        _bulletDamage = damage;
    }
}
