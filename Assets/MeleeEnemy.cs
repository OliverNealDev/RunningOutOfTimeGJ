using Unity.AI.Navigation;
using UnityEngine;

public class MeleeEnemy : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rb;
    

    [SerializeField] private float speed = 100f;
    
    private enum meleeEnemyState {/*Patrol, */Chasing, Attacking}

    [SerializeField] private meleeEnemyState currentMeleeEnemyState;
    
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
        switch (currentMeleeEnemyState)
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
            case meleeEnemyState.Chasing:
                if (agent.destination != Playertarget.position)
                {
                    agent.destination = Playertarget.position; 
                    
                    if (Vector3.Distance(transform.position, Playertarget.position) <= 2f)
                    {
                        currentMeleeEnemyState = meleeEnemyState.Attacking;
                        agent.velocity = Vector3.zero;
                    }
                    else
                    {
                        Vector3 direction = (agent.destination - transform.position).normalized;
                        agent.velocity = direction * speed;
                        
                        Debug.Log("making my way to yo ass");
                    }
                }
                break;
            case meleeEnemyState.Attacking:
                if (Vector3.Distance(transform.position, Playertarget.position) <= 2f)
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
                    currentMeleeEnemyState = meleeEnemyState.Chasing;
                }
                break;
        }

        if (currentMeleeEnemyState == meleeEnemyState.Attacking)
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
