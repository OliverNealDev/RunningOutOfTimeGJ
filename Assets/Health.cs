using UnityEditor;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxMealth = 100f;
    private float currentHealth;
    
    private float timeSinceLastHit;
    private float regenDelay = 3f;
    private float regenRate = 10f;

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
        }
    }
    
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        timeSinceLastHit = 0f;
        
        Instantiate(damageNumberPrefab, transform.position + damageNumberOffset, Quaternion.identity).GetComponentInChildren<DamageNumber>().Initialise(gameObject, damage);
        
        if (currentHealth <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
