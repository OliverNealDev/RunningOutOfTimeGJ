using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxMealth = 100f;
    private float currentHealth;

    [SerializeField] private bool isPlayer = false;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private List<AudioClip> hitSounds = new();
    
    private float timeSinceLastHit;
    private float regenDelay = 5f;
    private float regenRate = 5f;
    
    [SerializeField] private VisualEffectAsset electricityEffect;

    private GameObject damageNumberPrefab;
    [SerializeField] private Vector3 damageNumberOffset = Vector3.up;
    private float largeReactHealthPercent = 0.15f;

    void Start()
    {
        currentHealth = maxMealth;
        
        damageNumberPrefab = Resources.Load("DamageNumbers") as GameObject;
        
        if (animator == null) animator = gameObject.GetComponent<Animator>();
        if (audioSource == null) audioSource = gameObject.GetComponent<AudioSource>();
    }

    void Update()
    {
        timeSinceLastHit += Time.deltaTime;
        
        if (timeSinceLastHit >= regenDelay && currentHealth < maxMealth)
        {
            currentHealth += Mathf.Clamp(regenRate * Time.deltaTime, 0f, maxMealth);
            if (isPlayer)
            {
                AbilityUIController.Instance.UpdateHealth(currentHealth, maxMealth);
            }
        }
    }
    
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (isPlayer) AbilityUIController.Instance.UpdateHealth(currentHealth, maxMealth);

        if (hitSounds.Count > 0)
        {
            audioSource.PlayOneShot(hitSounds[Random.Range(0, hitSounds.Count)], AudioManager.Instance.sfxSource.volume);
        }

        if (animator != null)
        {
            if (damage >= maxMealth * largeReactHealthPercent)
            {
                animator.SetTrigger("largeDamage");
            }
            else
            {
                animator.SetTrigger("smallDamage");
            }
        }
        
        timeSinceLastHit = 0f;
        
        Instantiate(damageNumberPrefab, transform.position + damageNumberOffset, Quaternion.identity).GetComponentInChildren<DamageNumber>().Initialise(gameObject, damage);
        
        if (currentHealth <= 0f)
        {
            if (isPlayer)
            {
                SceneManager.LoadScene(0);
            }
            else
            {
                LevelManager.Instance.EnemyDied();
            }
            
            Destroy(gameObject);
        }
    }

    public void Heal(float healAmount)
    {
        currentHealth += healAmount;
        
        if (isPlayer) AbilityUIController.Instance.UpdateHealth(currentHealth, maxMealth);
    }

    Coroutine electricityCoroutine;
    VisualEffect activeElectricityEffect;
    public void StartElectricityEffect(float duration)
    {
        if (electricityCoroutine != null)
        {
            StopCoroutine(electricityCoroutine);
        }
        electricityCoroutine = StartCoroutine(ElectricityEffect(duration));
    }

    private IEnumerator ElectricityEffect(float duration)
    {
        if (activeElectricityEffect == null)
        {
            activeElectricityEffect = gameObject.AddComponent<VisualEffect>();
            activeElectricityEffect.visualEffectAsset = electricityEffect;
            activeElectricityEffect.SetFloat("Speed", 6);
        }
        
        int tickAmount = (int)(duration / 0.5f);

        for (int i = 0; i < tickAmount; i++)
        {
            yield return new WaitForSeconds(0.5f);
            TakeDamage(maxMealth * 0.085f);
        }
        electricityCoroutine = null;
        Destroy(activeElectricityEffect);
        activeElectricityEffect = null;
    }
}
