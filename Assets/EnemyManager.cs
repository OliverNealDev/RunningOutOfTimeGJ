using Unity.Mathematics;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject meleeEnemyPrefab;
    
    public GameObject rangedEnemyPrefab;
    public GameObject stomperEnemyPrefab; // does these stomps or attacks that hit everything around it
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // test
    }

    // Update is called once per frame
    void SpawnRoundOne()
    {
        Instantiate(meleeEnemyPrefab, transform.position, quaternion.identity);
    }
    
    void SpawnRoundTwo()
    {
        Instantiate(meleeEnemyPrefab, transform.position, quaternion.identity);
    }
    
    void SpawnRoundThree()
    {
        Instantiate(meleeEnemyPrefab, transform.position, quaternion.identity);
    }
    
    void SpawnRoundFour()
    {
        Instantiate(meleeEnemyPrefab, transform.position, quaternion.identity);
    }
    
    void SpawnRoundFive()
    {
        Instantiate(meleeEnemyPrefab, transform.position, quaternion.identity);
    }
}
