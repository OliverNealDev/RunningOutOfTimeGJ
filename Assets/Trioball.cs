using UnityEngine;

public class Trioball : MonoBehaviour
{
    private Rigidbody rigidBody;

    private int ballCount = 3;
    
    [SerializeField] private float projectileSpeed = 15f;

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
