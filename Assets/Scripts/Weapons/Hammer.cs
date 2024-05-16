using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hammer : MonoBehaviour
{
    private InputManager _inputManager;
    private Animator _hammerAnimator;
    private BoxCollider _hammerCollider;
    
    void Start()
    {
        _inputManager = InputManager.Instance;
        _hammerAnimator = GetComponent<Animator>();
        _hammerCollider = GetComponent<BoxCollider>();
        _hammerCollider.enabled = false;
    }

    void Update()
    {
        if (_inputManager.PlayerShotRevolver())
        {
            _hammerAnimator.SetTrigger("HitHammer");
            StartCoroutine(HitHammer());
        }
    }
    
    private IEnumerator HitHammer()
    {
        _hammerCollider.enabled = true;
        yield return new WaitForSeconds(0.5f);
        _hammerCollider.enabled = false;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"collider hit {other.gameObject.name}");
        if (other.gameObject.CompareTag("DestructableWall"))
        {
            Destroy(other.gameObject);
            var particles = Instantiate(Resources.Load<GameObject>("Prefabs/DestructParticles"), other.transform.position, Quaternion.identity);
            Destroy(particles, 2.2f);
        }
    }

}
