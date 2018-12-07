using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private Character _player;
    [SerializeField] private List<Transform> _spawnPoints;
    [SerializeField] private float _minSpawnRate;
    [SerializeField] private float _maxSpawnRate;
    [SerializeField] private Enemy2 _enemyPrefab;
    public List<Enemy2> EnemiesAlive { get; private set; }
    [SerializeField] private int _maxEnemiesAlive = 3;
    private Transform _lastSpawnLocation;

    private void Awake()
    {
        EnemiesAlive = new List<Enemy2>();
        
        if (_player)
            _player.OnPlayerDeath += (() => { CancelInvoke("SpawnEnemy"); });
    }

    private void Start()
    {
        Invoke("SpawnEnemy", Random.Range(_minSpawnRate, _maxSpawnRate));
    }

    private void SpawnEnemy()
    {
        if (EnemiesAlive.Count < _maxEnemiesAlive)
        {
            var randIndex = Random.Range(0, _spawnPoints.Count);

            EnemiesAlive.Add(Instantiate(_enemyPrefab, _spawnPoints[randIndex].position, Quaternion.identity));
            EnemiesAlive[EnemiesAlive.Count-1].OnDeath += RemoveEnemy;
            if (_player)
                EnemiesAlive[EnemiesAlive.Count-1].OnDeath += _player.ScorePoint;
        
            if (_lastSpawnLocation != null)
                _spawnPoints.Add(_lastSpawnLocation);
        
            _lastSpawnLocation = _spawnPoints[randIndex];
            _spawnPoints.RemoveAt(randIndex);
        }
        Invoke("SpawnEnemy", Random.Range(_minSpawnRate, _maxSpawnRate));
    }

    private void RemoveEnemy(Enemy2 enemy)
    {
        EnemiesAlive.Remove(enemy);
    }
}