using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;      
    public float spawnRate = 2.0f;
    public float spawnRangeX = 300f;
    public float maxY = 100f;
    public float minY = 3f;
    public int maxEnemies = 30;
    private Transform playerTransform;


  private float nextSpawnTime;

    private void Awake()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }
  private void Update()
    {
        if (Time.time > nextSpawnTime)
        {
            if(GameManager.instance.GetCurrEnemies() < maxEnemies)
                SpawnEnemy();
            nextSpawnTime = Time.time + spawnRate;
        }
    }

    private void SpawnEnemy()
    {
        float xSpawnPos = Random.Range(playerTransform.position.x - spawnRangeX, playerTransform.position.x + spawnRangeX);
        float randomWeight = (Random.Range(0f, 1f) * Random.Range(0f, 1f));  // Square the random value to weight it
        float ySpawnPos = Mathf.Lerp(minY, maxY, randomWeight);
        Instantiate(enemyPrefab, new Vector3(xSpawnPos, ySpawnPos, 0), Quaternion.identity);
        GameManager.instance.EnemySpawned();
    }
}

