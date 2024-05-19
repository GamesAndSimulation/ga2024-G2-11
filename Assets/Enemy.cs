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
    [SerializeField] private float patrollingSpeed = 3.5f;
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
                Chase();
                break;
            case EnemyState.Attack:
                EnemyStateText.text = "Attack";
                Attack();
                break;
        }
    }
    
    private void Patrol()
    {
        // If the agent is currently moving
        if (_agent.velocity.magnitude > Vector3.zero.magnitude || _isWaitingNewPosition)
        {
            return;
        }

        _isWaitingNewPosition = true;
        _animator.SetBool("Ideling", true);
        Invoke(nameof(SetNewRandomDestination), 4f);
    }

    private void Chase()
    {
        _agent.destination = player.position;
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
        _isWaitingNewPosition = false;
        _agent.speed = patrollingSpeed;
        _animator.SetBool("Ideling", false);
    }
}
