using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombSpawner : MonoBehaviour
{
	[SerializeField] private Character _player;
	[SerializeField] private List<Transform> _spawnPoints;
	[SerializeField] private float _minSpawnRate;
	[SerializeField] private float _maxSpawnRate;
	[SerializeField] private CollectableBomb _collectablePrefab;
	[SerializeField] private int _maxBombsSpawned = 3;
	private int _bombsCount = 0;
	private Transform _lastSpawnLocation;

	private void Awake()
	{
		if (_player)
			_player.OnPickupBombs += RemoveBomb;
	}

	private void Start()
	{
		Invoke("SpawnBombs", Random.Range(_minSpawnRate, _maxSpawnRate));
	}

	private void SpawnBombs()
	{
		if (_bombsCount < _maxBombsSpawned)
		{
			var randIndex = Random.Range(0, _spawnPoints.Count);

			Instantiate(_collectablePrefab, _spawnPoints[randIndex].position, Quaternion.identity);
			_bombsCount++;
			
        
			if (_lastSpawnLocation != null)
				_spawnPoints.Add(_lastSpawnLocation);
        
			_lastSpawnLocation = _spawnPoints[randIndex];
			_spawnPoints.RemoveAt(randIndex);
		}
		Invoke("SpawnBombs", Random.Range(_minSpawnRate, _maxSpawnRate));
	}

	private void RemoveBomb()
	{
		_bombsCount--;
	}
}
