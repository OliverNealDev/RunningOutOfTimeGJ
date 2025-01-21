using UnityEngine;

public class Trioball : MonoBehaviour
{
    private Rigidbody rigidBody;
    
    [SerializeField] private float lifetime = 10f;

    private int ballCount = 3;
    
    [SerializeField] private float projectileSpeed = 15f;

    void Start()
    {
        Invoke(nameof(Timeout), lifetime);    
    }

    void Timeout()
    {
        Destroy(gameObject);
    }
    
    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * projectileSpeed);
        transform.RotateAround(transform.position, transform.forward, 250f * Time.deltaTime);
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.collider.TryGetComponent(out Health health))
        {
            health.TakeDamage(25f);
        }

        Collider childCollider = other.GetContact(0).thisCollider;
        Destroy(childCollider.gameObject);
        ballCount--;

        if (ballCount <= 0)
        {
            Destroy(gameObject);
        }
    }
}
