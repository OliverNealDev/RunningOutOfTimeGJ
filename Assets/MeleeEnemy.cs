using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MeleeEnemy : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private Collider attackCollider;
    [SerializeField] private AnimationClip attackAnim;
    [SerializeField] private Collider stompCollider;
    [SerializeField] private AnimationClip stompAnimation;
    
    private Transform player;
    private float attackRange = 2f;
    private float attackCooldown = 2f;
    private int damage = 10;

    private NavMeshAgent agent;
    private float lastAttackTime;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (player == null)
        {
            player = FindAnyObjectByType<PlayerController>().transform;
        }
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange && Time.time > lastAttackTime + attackCooldown)
        {
            StartCoroutine(Attack());
            lastAttackTime = Time.time;
        }
        else
        {
            ChasePlayer();
        }
        
        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    IEnumerator Attack()
    {
        Debug.Log("Enemy attacks!");
        animator.SetTrigger("Attack");
        attackCollider.enabled = true;
        
        yield return new WaitForSeconds(attackAnim.length);
        
        attackCollider.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform == player && other.TryGetComponent(out Health health))
        {
            health.TakeDamage(damage);
        }
    }
}
