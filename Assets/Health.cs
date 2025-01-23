using UnityEditor;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxMealth = 100f;
    private float currentHealth;

    [SerializeField] private bool isPlayer = false;
    
    private float timeSinceLastHit;
    private float regenDelay = 5f;
    private float regenRate = 5f;

    private GameObject damageNumberPrefab;
    [SerializeField] private Vector3 damageNumberOffset = Vector3.up;

    void Start()
    {
        currentHealth = maxMealth;

        damageNumberPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/DamageNumbers.prefab");
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
        timeSinceLastHit = 0f;
        
        Instantiate(damageNumberPrefab, transform.position + damageNumberOffset, Quaternion.identity).GetComponentInChildren<DamageNumber>().Initialise(gameObject, damage);
        
        if (currentHealth <= 0f)
        {
            LevelManager.Instance.EnemyDied();
            
            Destroy(gameObject);
        }
    }
}
