using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private GameObject player;
    private NavMeshAgent agent;
    private Animator animator;
    
    private float defaultStoppingDistance;
    
    private float damage;
    [SerializeField] private float attackCooldown = 3;
    [SerializeField] private float attackRange = 3f;
    [SerializeField] private Collider swordCollider;
    [SerializeField] private float swordDamage = 27;
    [SerializeField] private Collider stompCollider;
    [SerializeField] private float stompDamage = 53;
    private float timePassed;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        
        timePassed = attackCooldown;
        
        defaultStoppingDistance = agent.stoppingDistance;
    }

    void Update()
    {
        animator.SetFloat("Speed", agent.velocity.magnitude / agent.speed);

        if (timePassed >= attackCooldown && Vector3.Distance(player.transform.position, transform.position) <= attackRange)
        {
            float attack = Random.Range(0f, 10f);

            if (attack < 5)
            {
                animator.SetTrigger("attack");
                timePassed = 0;
            }
            else
            {
                animator.SetTrigger("jumpAttack");
                timePassed = 0;
            }
        }
        else
        {
            agent.SetDestination(player.transform.position);
        }
        
        timePassed += Time.deltaTime;
    }

    public void SwordColliderOn()
    {
        swordCollider.enabled = true;
        damage = swordDamage;
    }

    public void SwordColliderOff()
    {
        swordCollider.enabled = false;
    }

    public void StompCollider(float duration)
    {
        StartCoroutine(StompEnum(duration));
    }

    private IEnumerator StompEnum(float duration)
    {
        stompCollider.enabled = true;
        damage = stompDamage;
        
        yield return new WaitForSeconds(duration);
        
        stompCollider.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Health health) && other.gameObject == player)
        {
            health.TakeDamage(damage);
        }
    }

    public void StopMovement()
    {
        agent.stoppingDistance = 9999f;
    }

    public void StartMovement()
    {
        agent.stoppingDistance = defaultStoppingDistance;
    }
}
