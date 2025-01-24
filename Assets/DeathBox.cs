using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DeathBox : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Health health))
        {
            health.TakeDamage(99999f);
        }
    }
}
