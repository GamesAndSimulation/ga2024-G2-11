using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFov : MonoBehaviour
{
    public enum EnemyType {Soldier, Turret}
    
    public EnemyType enemyType;
    
    public float radius;
    [Range(0, 360)]
    public float angle;
    
    private Enemy _enemyScript;
    private Turret _turretScript;
    private Animator _enemyAnim;
    [SerializeField] private AnimationClip _drawAxeClip;
    [SerializeField] private Transform povTransform;
    private Vector3 playerTransform;

    public LayerMask targetMask;
    public LayerMask obstacleMask;
    
    void Start()
    {
        if(enemyType == EnemyType.Soldier)
            _enemyScript = transform.GetComponent<Enemy>();
        else if(enemyType == EnemyType.Turret)
            _turretScript = transform.GetComponent<Turret>();
        _enemyAnim = transform.GetComponentInChildren<Animator>();
        if (povTransform == null)
        {
            povTransform = transform;
        }
        playerTransform = GameManager.Instance.GetPlayerPosition();
        StartCoroutine(FOVRoutine());
    }
    
    void Update()
    {
        if (GameManager.Instance.gamePaused)
        {
            _enemyAnim.speed = 0;
        }
        
    }

    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);
        while (true)
        {
            yield return wait;
            if(GameManager.Instance.gamePaused)
                continue;
            if (FieldOfViewCheck(angle))
            {
                switch (enemyType)
                {
                    case EnemyType.Soldier:
                        StartCoroutine(StartChaseRoutine());
                        yield break;
                    case EnemyType.Turret:
                        Debug.Log("Turret : Player in sight!");
                        _turretScript.SetCanShoot(true);
                        break;   
                }
            }
            else if (enemyType == EnemyType.Turret)
                _turretScript.SetCanShoot(false);
        }
    }

    private IEnumerator StartChaseRoutine()
    {
        _enemyAnim.SetTrigger("SpottedPlayer");

        yield return new WaitForSeconds(_drawAxeClip.length);
        
        _enemyScript.currentEnemyState = Enemy.EnemyState.Chase;
        
        yield return null;
    }
    
    public bool FieldOfViewCheck(float angle)
    {
        Collider[] rangeChecks = Physics.OverlapSphere(povTransform.position, radius, targetMask);
        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - povTransform.position).normalized;
            if (Vector3.Angle(povTransform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(povTransform.position, target.position);
                if (!Physics.Raycast(povTransform.position, directionToTarget, distanceToTarget, obstacleMask))
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
    
    //draw field of view gizmo
    private void OnDrawGizmos()
    {
        if (povTransform == null)
            povTransform = transform;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(povTransform.position, 1f);
        Vector3 fovLine1 = Quaternion.AngleAxis(angle/ 2, povTransform.up) * povTransform.forward * radius;
        Vector3 fovLine2 = Quaternion.AngleAxis(-angle/ 2, povTransform.up) * povTransform.forward * radius;

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(povTransform.position, fovLine1);
        Gizmos.DrawRay(povTransform.position, fovLine2);

        Gizmos.color = Color.green;
        Gizmos.DrawRay(povTransform.position, povTransform.forward * radius);

        // Assuming you have a reference to the player's transform
        Vector3 directionToPlayer = (playerTransform - povTransform.position).normalized;
        float distanceToPlayer = Vector3.Distance(povTransform.position, playerTransform);

        RaycastHit hit;
        if (Physics.Raycast(povTransform.position, directionToPlayer, out hit, distanceToPlayer))
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(hit.point, 0.2f); // Draws a small sphere at the point of collision
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(povTransform.position, playerTransform);
        }
    }

}

