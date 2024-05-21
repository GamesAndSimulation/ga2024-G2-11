using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFov : MonoBehaviour
{
    public float radius;
    [Range(0, 360)]
    public float angle;
    
    private GameObject player;
    private Enemy _enemyScript;
    private Animator _enemyAnim;
    [SerializeField] private AnimationClip _drawAxeClip;

    public LayerMask targetMask;
    public LayerMask obstacleMask;
    
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        _enemyScript = transform.GetComponent<Enemy>();
        _enemyAnim = transform.GetComponentInChildren<Animator>();
        StartCoroutine(FOVRoutine());
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
            _enemyAnim.SetTrigger("Attack");
    }

    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);
        while (true)
        {
            yield return wait;
            if (FieldOfViewCheck())
            {
                StartCoroutine(StartChaseRoutine());
                yield break;
            }
        }
    }

    private IEnumerator StartChaseRoutine()
    {
        _enemyAnim.SetTrigger("SpottedPlayer");

        yield return new WaitForSeconds(_drawAxeClip.length);
        
        _enemyScript.currentEnemyState = Enemy.EnemyState.Chase;
        
        yield return null;
    }
    
    bool FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);
        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);
                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleMask))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

}
