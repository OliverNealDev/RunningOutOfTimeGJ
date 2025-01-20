using System.Collections.Generic;
using UnityEngine;

public class FireAoE : MonoBehaviour
{
    private float tickRate = 1f;
    private float timeSinceLastHit;
    private float lifetime = 10f;
    
    [SerializeField] private List<Health> healths = new List<Health>();

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
        timeSinceLastHit += Time.deltaTime;
        if (timeSinceLastHit >= tickRate)
        {
            LogicTickRate();
        }
    }

    void LogicTickRate()
    {
        foreach (Health health in healths)
        {
            health.TakeDamage(10f);
        }
    }
    
    /*void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Health health))
        {
            healths.Add(health);
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Health health))
        {
            healths.Remove(health);
        }
    }*/
}
