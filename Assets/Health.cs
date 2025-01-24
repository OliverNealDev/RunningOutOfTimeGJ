using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxMealth = 100f;
    private float currentHealth;

    [SerializeField] private bool isPlayer = false;
    [SerializeField] private Animator animator;
    
    private float timeSinceLastHit;
    private float regenDelay = 5f;
    private float regenRate = 5f;

    private GameObject damageNumberPrefab;
    [SerializeField] private Vector3 damageNumberOffset = Vector3.up;
    private float largeReactHealthPercent = 0.15f;

    void Start()
    {
        currentHealth = maxMealth;
        
        damageNumberPrefab = Resources.Load("DamageNumbers") as GameObject;
        
        if (animator == null) animator = gameObject.GetComponent<Animator>();
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
        int tickAmount = (int)(duration / 0.5f);

        for (int i = 0; i < tickAmount; i++)
        {
            yield return new WaitForSeconds(0.5f);
            TakeDamage(maxMealth * 0.07f);
        }
        electricityCoroutine = null;
    }
    
}
