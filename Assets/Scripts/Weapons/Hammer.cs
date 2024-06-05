using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hammer : MonoBehaviour
{
    private InputManager _inputManager;
    private Animator _hammerAnimator;
    private BoxCollider _hammerCollider;
    private int brokenRockPieces = 0;

    [SerializeField] private AudioClip[] rockSounds;
    [SerializeField] private int rockSoundNumInterval = 0;
    
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
        
        if (other.gameObject.CompareTag("DestructableWall"))
        {
            brokenRockPieces++;
            if (brokenRockPieces % rockSoundNumInterval == 0)
                AudioManager.Instance.PlaySoundAtPosition(rockSounds[UnityEngine.Random.Range(0, rockSounds.Length)], other.transform.position, 4f);
            Destroy(other.gameObject);
            var particles = Instantiate(Resources.Load<GameObject>("Prefabs/DestructParticles"), other.transform.position, Quaternion.identity);
            Destroy(particles, 2.2f);
        }
    }

}
