using Unity.AI.Navigation;
using UnityEngine;

public class RangedEnemey : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rb;
    

    [SerializeField] private float speed = 100f;
    
    private enum rangedEnemyState {/*Patrol, */Positioning, Attacking}

    [SerializeField] private rangedEnemyState currentRangedEnemyState;
    
    //Navmesh
    [SerializeField] private NavMeshSurface surface;
    [SerializeField] private Transform Playertarget;
    
    
    UnityEngine.AI.NavMeshAgent agent;
    
    //Patrol
    private Vector3 targetPosition;
    
    //Attacking
    private float attackTimer;
    [SerializeField] private float attackCooldown;
    [SerializeField] private float damage;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        Playertarget = GameObject.FindGameObjectWithTag("Player").transform; // Oliver - Bit of a hack but this works for now
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    void FixedUpdate() // changed to sync with physics tickrate because velocity calculaations
    {
        switch (currentRangedEnemyState)
        {
            /*case meleeEnemyState.Patrol:
                if (agent.pathStatus != UnityEngine.AI.NavMeshPathStatus.PathComplete)
                {
                    Vector3 direction = (agent.destination - transform.position).normalized;
                    agent.velocity = direction * speed;
                }
                else if (agent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathComplete)
                {
                    agent.destination = new Vector3(Random.Range(-10.0f, 10.0f), transform.position.y, Random.Range(-10.0f, 10.0f));
                    
                    Vector3 direction = (agent.destination - transform.position).normalized;
                    agent.velocity = direction * speed;
                }
                break;*/
            case rangedEnemyState.Positioning:
                if (agent.destination != Playertarget.position)
                {
                    if (Vector3.Distance(transform.position, Playertarget.position) >= 5f && Vector3.Distance(transform.position, Playertarget.position) <= 10f)
                    {
                        currentRangedEnemyState = rangedEnemyState.Attacking;
                        agent.velocity = Vector3.zero;
                    }
                    else if (Vector3.Distance(transform.position, Playertarget.position) >= 10f)
                    {
                        agent.destination = Playertarget.position; 
                        
                        Vector3 direction = (agent.destination - transform.position).normalized;
                        agent.velocity = direction * speed;
                    }
                    else
                    {
                        Vector3 Awaydirection = (transform.position - Playertarget.position).normalized;
                        agent.destination = Playertarget.position + Awaydirection * 10;
                        
                        Vector3 direction = (agent.destination - transform.position).normalized;
                        agent.velocity = direction * speed;
                    }
                }
                break;
            case rangedEnemyState.Attacking:
                if (Vector3.Distance(transform.position, Playertarget.position) >= 5f && Vector3.Distance(transform.position, Playertarget.position) <= 10f)
                {
                    attackTimer += Time.fixedDeltaTime;
                    if (attackTimer >= attackCooldown)
                    {
                        attackTimer -= attackCooldown;
                        Playertarget.GetComponent<Health>().TakeDamage(damage);
                    }
                }
                else
                {
                    currentRangedEnemyState = rangedEnemyState.Positioning;
                }
                break;
        }

        if (currentRangedEnemyState == rangedEnemyState.Attacking)
        {
            transform.LookAt(Playertarget);
        }
        else
        {
            transform.LookAt(agent.destination);
        }
        
        Vector3 currentVelocity = rb.linearVelocity;
        //rb.linearVelocity = Vector3.SmoothDamp(rb.linearVelocity, transform.forward * speed, ref currentVelocity, speed);
        //rb.linearVelocity = Vector3.forward * speed;
        if (currentVelocity.magnitude > 1f)
        {
            animator.SetFloat("Speed", currentVelocity.magnitude);
        }
    }

    /*void Update()
    {
        if (agent.destination != Playertarget.position)
        {
            transform.LookAt(agent.destination);
        }
        else
        {
            transform.LookAt(Playertarget.position);
        }
    }*/
}
