using System.Collections;using DG.Tweening;
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
    
    [Header("Attack")]
    public float AttackDistance;
    public float Damage;
    public float AttackCheckAngle;
    public float AttackDelayTime;
    
    private NavMeshAgent _agent;
    private Animator _animator;
    private Transform _player;
    private EnemyFov _enemyFov;
    private bool _attacking;

    [Header("AI")]
    
    private bool _isWaitingNewPosition;
    [SerializeField] private float walkRadius = 5f;
    [SerializeField] private float patrollingSpeed;
    [SerializeField] private float chaseSpeed = 5f;
    
    [Header("Sound")]
    public AudioClip[] footstepSounds;
    public AudioClip[] playerSpottedYellSounds;
    public AudioClip meleeSwingSound;
    public float footstepSoundCooldownTime = 0.43f;
    private float footstepSoundCooldownTimer;
    
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _enemyFov = GetComponent<EnemyFov>();
        _animator = GetComponentInChildren<Animator>();
        currentEnemyState = EnemyState.Patrol;
        _player = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {

        if (GameManager.Instance.gamePaused)
        {
            _agent.isStopped = true;
            _animator.speed = 0;
            return;
        }
        
        footstepSoundCooldownTimer -= Time.deltaTime;

        if (!_agent.isStopped && footstepSoundCooldownTimer <= 0)
        {
            AudioManager.Instance.PlaySoundAtPosition(footstepSounds[Random.Range(0, footstepSounds.Length)], transform.position);
            footstepSoundCooldownTimer = footstepSoundCooldownTime;
        }
        
        _agent.isStopped = false;
        _animator.speed = 1;
        
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
                if(!_attacking)
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
        if(Vector3.Distance(transform.position, _player.position) < AttackDistance && _enemyFov.FieldOfViewCheck(AttackCheckAngle))
            currentEnemyState = EnemyState.Attack;
    }

    private void Attack()
    {
        if (_attacking || _animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            Debug.Log("Already attacking or attack animation is playing.");
            return;
        }

        Debug.Log("Starting Attack");
        _attacking = true;

        _agent.isStopped = true;
        _animator.SetTrigger("Attack");
        _player.GetComponent<PlayerStats>().TakeDamage(Damage);
        AudioManager.Instance.PlaySoundAtPosition(meleeSwingSound, transform.position);
        Invoke(nameof(StopAttacking), AttackDelayTime);
    }

    private void StopAttacking()
    {
        _attacking = false;
        currentEnemyState = EnemyState.Chase;
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
            _player.GetComponent<PlayerStats>().AddEnemyKill();
            _agent.isStopped = true;
            _animator.SetTrigger("Die");
            var body = transform.Find("Body").transform;
            body.DOMoveY(body.position.y - 0.535f, 0.5f);
            enabled = false;
        }
    }
}