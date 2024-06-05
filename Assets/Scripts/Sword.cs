using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    
    private InputManager inputManager;
    private Animator _swordAnimator;
    public AnimationClip _swingAnimationClip;
    
    [Header("Sword Stats")]
    public float Damage;

    public float Range;
    
    
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
            Invoke(nameof(Attack), 0.25f);
        }
    }
    
    void Attack()
    {
        Vector3 fowardVec = GameManager.Instance.GetCameraForward();
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, fowardVec, out hit, Range))
        {
            Debug.Log($"Sword Hit {hit.transform.name}");
            if (hit.transform.CompareTag("Enemy"))
            {
                var hitParticles = Instantiate( Resources.Load<GameObject>("Prefabs/SwordHitParticles"), hit.point, Quaternion.identity);
                Destroy(hitParticles, 1f);
                var enemyScript = hit.transform.GetComponent<Enemy>();
                if(enemyScript.enabled)
                    enemyScript.TakeDamage(Damage);
            }
        }
    }
    
}
