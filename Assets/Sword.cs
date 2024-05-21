using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    
    private InputManager inputManager;
    private Animator _swordAnimator;
    public AnimationClip _swingAnimationClip;
    
    private float _attackTime;
    
    void Start()
    {
        inputManager = InputManager.Instance;
        _swordAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        if(GameManager.Instance.inPuzzleMode)
            return;
        
        _attackTime -= Time.deltaTime;
        if (inputManager.PlayerSwingedSword() && _attackTime <= 0)
        {
            _swordAnimator.SetTrigger("SwordAttack");
            _attackTime = _swingAnimationClip.length;
        }
    }
    
}
