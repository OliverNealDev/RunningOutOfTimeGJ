using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxMealth = 100f;
    private float currentHealth;
    
    private float timeSinceLastHit;
    private float regenDelay = 3f;
    private float regenRate = 10f;

    void Start()
    {
        currentHealth = maxMealth;
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
        Debug.Log(damage + " " + gameObject.name);

        currentHealth -= damage;
        timeSinceLastHit = 0f;
        
        if (currentHealth <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
