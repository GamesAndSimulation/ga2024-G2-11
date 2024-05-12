using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hammer : MonoBehaviour
{
    private InputManager _inputManager;
    private Animator _hammerAnimator;
    
    void Start()
    {
        _inputManager = InputManager.Instance;
        _hammerAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        if (_inputManager.PlayerShotRevolver())
        {
            _hammerAnimator.SetTrigger("HitHammer");
        }
        
    }
}
