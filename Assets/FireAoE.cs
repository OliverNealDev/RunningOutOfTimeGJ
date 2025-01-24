using System;
using System.Collections.Generic;
using UnityEngine;

public class FireAoE : MonoBehaviour
{
    private float tickRate = 1f;
    private float lifetime = 10f;
    
    private float timeWhenLastHit;

    private List<Health> healths = new List<Health>();
    
    private AudioSource audioSource;
    
    void Start()
    {
        /*audioSource = GetComponent<AudioSource>();
        audioSource.volume = AudioManager.Instance.sfxSource.volume;
        audioSource.Play();*/
        
        Invoke(nameof(Timeout), lifetime);
        InvokeRepeating(nameof(ApplyDamage), 0, tickRate);
    }

    void Timeout()
    {
        Destroy(gameObject);
    }

    void ApplyDamage()
    {
        foreach (Health health in healths)
        {
            //This is the silly num check that's tells me to do a null check
            if (health != null)
            {
                health.TakeDamage(25f);
            }
        }
    }

    void OnTriggerEnter(Collider other)
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
    }
}
