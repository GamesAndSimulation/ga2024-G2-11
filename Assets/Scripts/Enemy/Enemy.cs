using System.Collections;
using DG.Tweening;
using TMPro;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    
    public enum EnemyState
    {
        Patrol,
        Chase,
        Attack
    }

    public EnemyState currentEnemyState;
    public NavMeshSurface surface;
    public TextMeshPro EnemyStateText;
    public float Health;
    public float AttackDistance;
    
    private NavMeshAgent _agent;
    private Animator _animator;
    private Transform _player;

    [Header("AI")]
    
    private bool _isWaitingNewPosition;
    [SerializeField] private float walkRadius = 5f;
    [SerializeField] private float patrollingSpeed;
    [SerializeField] private float chaseSpeed = 5f;
    
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponentInChildren<Animator>();
        currentEnemyState = EnemyState.Patrol;
        _player = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        
        switch (currentEnemyState)
        {
            case EnemyState.Patrol:
                EnemyStateText.text = "Patrolling";
                Patrol();
                break;
            case EnemyState.Chase:
                EnemyStateText.text = "Chase";
                Chase(_player);
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
        _agent.isStopped = false;
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
        _agent.isStopped = false;
        _agent.destination = target.position;
        _agent.speed = chaseSpeed;
        if(Vector3.Distance(transform.position, _player.position) < AttackDistance)
            currentEnemyState = EnemyState.Attack;
    }

    private void Attack()
    {
        //if current clip is attack clip
        Debug.Log($"_animator is in attack state {_animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")}");
        if(_animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
            return;
        
        _animator.SetTrigger("Attack");
        _agent.isStopped = true;
        
        // Look at player
        //Vector3 direction = _player.position - transform.position;
        //transform.rotation = Quaternion.LookRotation(direction);
        
        // Damage Player
        
        // Change state to chase
        if(Vector3.Distance(transform.position, _player.position) > AttackDistance)
            currentEnemyState = EnemyState.Chase;
        //currentEnemyState = EnemyState.Chase;
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
    
    public void TakeDamage(float damage)
    {
        if (Health <= 0)
            return;
        Health -= damage;
        currentEnemyState = EnemyState.Chase;
        if (Health <= 0)
        {
            Debug.Log("Enemy died!");
            _agent.isStopped = true;
            _animator.SetTrigger("Die");
            var body = transform.Find("Body").transform;
            body.DOMoveY(body.position.y - 0.535f, 0.5f);
            enabled = false;
        }
    }
}