using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] List<GameObject> enemies = new List<GameObject>();
    [SerializeField] private float spawnDistance = 22f;
    [SerializeField] private float spawnRate;
    [SerializeField] private float minSpawnRate;
    [SerializeField] private float spawnRateDecrement;
    [SerializeField] private int decrementAfterXEnemies;
    
    private float spawnedEnemies;

    private void Start()
    {
        if (enemies.Count == 0)
        {
            Debug.LogWarning("No enemies found");
        }
        else
        {
            InvokeRepeating(nameof(SpawnEnemies), 0, spawnRate);
        }
    }

    private void SpawnEnemies()
    {
        Vector3 spawnLocation = Random.insideUnitSphere * spawnDistance;
        
        spawnLocation.y = 0f;
        
        GameObject enemy = Instantiate(enemies[Random.Range(0, enemies.Count)], spawnLocation, Quaternion.identity);
        
        spawnedEnemies++;

        if (spawnedEnemies % decrementAfterXEnemies == 0 && spawnRate > minSpawnRate)
        {
            spawnRate -= spawnRateDecrement;
            CancelInvoke(nameof(SpawnEnemies));
            InvokeRepeating(nameof(SpawnEnemies), spawnRate, spawnRate);
        }
    }
}
