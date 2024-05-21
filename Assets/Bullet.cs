using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float _bulletDamage;
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("Enemy"))
        {
            other.transform.GetComponent<Enemy>().TakeDamage(_bulletDamage);
            var particles = Instantiate(Resources.Load<GameObject>("Prefabs/SwordHitParticles"), other.GetContact(0).point, Quaternion.identity);
            Destroy(particles, 1f);
        }

        transform.GetComponent<Rigidbody>().isKinematic = true;
        transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
        transform.GetComponent<SphereCollider>().enabled = false;
        Destroy(gameObject, 2f);
    }

    public void SetDamage(float damage)
    {
        _bulletDamage = damage;
    }
}
