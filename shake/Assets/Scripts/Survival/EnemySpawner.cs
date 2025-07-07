using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
  [SerializeField]
  private Combat _enemyPrefab;

  [SerializeField]
  private List<Transform> _spawnPoints;

  [SerializeField]
  private LayerMask _enemyLayer;

  private WaveManager waveManager;

  private readonly List<Combat> listCreatedEnemies = new();

  public int AliveEnemies { get; private set; }

  public List<Transform> SpawnPoints => _spawnPoints;

  private void Awake()
  {
    waveManager = GetComponent<WaveManager>();
  }

  public void Spawn(int parCount)
  {
	AliveEnemies = parCount;

    StartCoroutine(TrySpawnEnemy(parCount));
  }

  public void OnEnemyDeath(Combat parEnemy)
  {
	AliveEnemies--;
    listCreatedEnemies.Remove(parEnemy);
    parEnemy.OnDied -= OnEnemyDeath;

    if (AliveEnemies <= 0)
      waveManager.StartNextWave();
  }

  private void SpawnEnemy(Vector3 parPosition)
  {
    Combat enemy = Instantiate(_enemyPrefab, parPosition, Quaternion.identity);
    enemy.OnDied += OnEnemyDeath;
    listCreatedEnemies.Add(enemy);
  }

  private IEnumerator TrySpawnEnemy(int parNumberEnemies)
  {
    WaitForSeconds waitForSeconds = new(0.25f);

    int maxAttempts = 10;
    int nearbyAttempts = 8;
    float checkRadius = 1f;

    for (int i = 0; i < parNumberEnemies; i++)
    {
      bool spawned = false;

      while (!spawned)
      {
        for (int j = 0; j < maxAttempts; j++)
        {
          Transform spawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Count)];

          if (!Physics.CheckSphere(spawnPoint.position, checkRadius, _enemyLayer))
          {
            SpawnEnemy(spawnPoint.position);
            spawned = true;
            break;
          }

          for (int n = 0; n < nearbyAttempts; n++)
          {
            Vector3 offset = Random.onUnitSphere;
            offset.y = 0;
            offset = offset.normalized * 2f;

            Vector3 nearbyPosition = spawnPoint.position + offset;

            if (!Physics.CheckSphere(nearbyPosition, checkRadius, _enemyLayer))
            {
              SpawnEnemy(nearbyPosition);
              spawned = true;
              break;
            }
          }

          if (!spawned)
            yield return null;
          else
            break;
        }
      }

      yield return waitForSeconds;
    }
  }
}