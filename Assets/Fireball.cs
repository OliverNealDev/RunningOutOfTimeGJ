using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Fireball : MonoBehaviour
{
    private float lifetime = 10f;
    
    [SerializeField] private GameObject aoePrefab;
    private float damage = 90f;
    private float projectileSpeed = 15f;
    
    [SerializeField] private LayerMask interactableLayer;
    
    void Start()
    {
        Invoke(nameof(Timeout), lifetime);
        GetComponent<Rigidbody>().linearVelocity = transform.forward * projectileSpeed;
    }

    void Timeout()
    {
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            return;
        }
        
        if (other.TryGetComponent(out Health health))
        {
            health.TakeDamage(damage);
        }
        
        Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 100f, interactableLayer);
        Instantiate(aoePrefab, hit.point, Quaternion.identity);
    
        Destroy(gameObject);
    }
}
