using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
  private EnemySpawner enemySpawner;

  public int CurrentWave { get; private set; }

  private void Awake()
  {
    enemySpawner = GetComponent<EnemySpawner>();
  }

  private void Start()
  {
    StartNextWave();
  }

  public void StartNextWave()
  {
    CurrentWave++;

    int enemyCount = 5 + (CurrentWave - 1) * 3;
    enemySpawner.Spawn(enemyCount);
  }
}