using UnityEngine;

public class MeleeEnemy : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rb;

    [SerializeField] private float speed = 100f;
    
    
    
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Vector3 currentVelocity = rb.linearVelocity;
        rb.linearVelocity = Vector3.SmoothDamp(rb.linearVelocity, transform.forward * speed, ref currentVelocity, speed);
        animator.SetFloat("Speed", currentVelocity.magnitude);
    }
}
