using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance; //singleton

    [SerializeField] private float _minSpawnTime;
    [SerializeField] private float _maxSpawnTime;
    [SerializeField] private float _maxEnemiesAlive;
    [SerializeField] private List<Transform> _spawnPoints;
    [SerializeField] private Enemy _enemyPrefab;
    public List<Enemy> SpawnedEnemies { get; private set; }

    public event Action<int> OnEnemyDeath;

    protected void Awake()
    {
        instance = this;
        SpawnedEnemies = new List<Enemy>();
    }

    protected void Start()
    {
        for (int i = 0; i < _maxEnemiesAlive; i++)
        {
            SpawnEnemy();
        }
    }

    protected void SpawnEnemy()
    {
        Vector3 spawnPos = _spawnPoints[UnityEngine.Random.Range(0, _spawnPoints.Count)].position;
        SpawnedEnemies.Add(Instantiate(_enemyPrefab, spawnPos, Quaternion.identity, transform));
    }

    public void EnemyKilled(Enemy enemy)
    {
        SpawnedEnemies.Remove(enemy);
        Invoke("SpawnEnemy", UnityEngine.Random.Range(_minSpawnTime, _maxSpawnTime));
        if (OnEnemyDeath != null)
            OnEnemyDeath(enemy.ScorePoints);
    }
}