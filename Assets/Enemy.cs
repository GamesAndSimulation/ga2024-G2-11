using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    
    public enum EnemyState
    {
        Patrol,
        Chase,
        Attack
    }

    public EnemyState currentEnemyState;
    public Transform player;
    public NavMeshSurface surface;
    public TextMeshPro EnemyStateText;
    
    private NavMeshAgent _agent;
    private Animator _animator;

    private bool _isWaitingNewPosition;
    [SerializeField] private float walkRadius = 5f;
    [SerializeField] private float patrollingSpeed;
    [SerializeField] private float chaseSpeed = 5f;
    
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponentInChildren<Animator>();
        currentEnemyState = EnemyState.Patrol;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            currentEnemyState = EnemyState.Chase;
        }
        
        switch (currentEnemyState)
        {
            case EnemyState.Patrol:
                EnemyStateText.text = "Patrolling";
                Patrol();
                break;
            case EnemyState.Chase:
                EnemyStateText.text = "Chase";
                Chase(player);
                break;
            case EnemyState.Attack:
                EnemyStateText.text = "Attack";
                Attack();
                break;
        }
        
        _animator.SetFloat("velocity", _agent.velocity.sqrMagnitude);
    }
    
    private void Patrol()
    {
        if(!IsAgentMoving(_agent) && !_isWaitingNewPosition)
            StartCoroutine(RandomDestinationWithDelay());
    }

    private IEnumerator RandomDestinationWithDelay()
    {
        _isWaitingNewPosition = true;
        yield return new WaitForSeconds(4f);
        SetNewRandomDestination();
        _isWaitingNewPosition = false;
    }

    public void Chase(Transform target)
    {
        _agent.destination = target.position;
        _agent.speed = chaseSpeed;
    }

    private void Attack()
    {
        
    }

    private void SetNewRandomDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
        randomDirection += transform.position;
        
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, walkRadius, 1);
        
        _agent.destination = hit.position;
        _animator.SetBool("isWalking", true);
        _agent.speed = patrollingSpeed;
    }
    
    bool IsAgentMoving(NavMeshAgent navMeshAgent)
    {
        if (navMeshAgent.pathPending) return true;

        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f)
            {
                return false;
            }
        }

        return true;
    }
}