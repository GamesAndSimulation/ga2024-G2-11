using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    
    public enum EnemyState
    {
        PATROL,
        CHASE,
        ATTACK
    }

    public EnemyState currentEnemyState;
    public Transform player;
    public NavMeshSurface surface;
    private NavMeshAgent agent;

    private bool _isWaitingNewPosition;
    [SerializeField] private float walkRadius = 5f;
    
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentEnemyState = EnemyState.PATROL;
    }

    void Update()
    {
        switch (currentEnemyState)
        {
            case EnemyState.PATROL:
                Patrol();
                break;
        }
    }

    private void Patrol()
    {
        // If the agent is currently moving
        if (agent.velocity.magnitude > Vector3.zero.magnitude || _isWaitingNewPosition)
        {
            return;
        }

        _isWaitingNewPosition = true;
        Invoke(nameof(SetNewRandomDestination), 4f);
    }

    private void SetNewRandomDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
        randomDirection += transform.position;
        
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, walkRadius, 1);
        
        agent.destination = hit.position;
        _isWaitingNewPosition = false;
    }
}
