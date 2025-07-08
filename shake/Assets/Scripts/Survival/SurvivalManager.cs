using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public sealed class SurvivalManager : MonoBehaviour
{
  private static SurvivalManager instance;

  [SerializeField] private SnakePart _snakePartPrefab;
  [SerializeField] private LayerMask _ignoreMask;
  [SerializeField] private LayerMask _mask;

  private int currentLevel = 1;
  private int currentDiamond = 0;
  private int currentEnergy = 0;

  private EnemySpawner enemySpawner;

  private List<DropItemBase> listDropItems = new();

  public event Action<int> OnChangeDiamond;
  public event Action<int, int> OnChangeEnergy;

  public static SurvivalManager Instance => instance;

  public int Goal => 2 * currentLevel;

  public int CurrentDiamond
  {
    get => currentDiamond;
    private set
    {
      currentDiamond = value;
      OnChangeDiamond?.Invoke(value);
    }
  }

  public int CurrentEnergy
  {
    get => currentEnergy;
    private set
    {
      currentEnergy = value;
      OnChangeEnergy?.Invoke(value, Goal);
    }
  }

  private void Awake()
  {
    enemySpawner = GetComponent<EnemySpawner>();

    if (instance != null && instance != this)
    {
      Destroy(this);
      return;
    }

    instance = this;

    GameManager.Instance.LevelManager.InitializeSurvivalMode(this);
  }

  private void Start()
  {
    CurrentDiamond = 0;
    CurrentEnergy = 0;
  }

  public void AddDropItem(DropItemBase parDropItem)
  {
    if (parDropItem == null)
      return;

    listDropItems.Add(parDropItem);
  }

  public void RemoveDropItem(DropItemBase parDropItem)
  {
    if (parDropItem == null)
      return;

    listDropItems.Remove(parDropItem);
  }

  public void KillEnemiesRadius(float parRadius, float parForce, int parDamage, Vector3 parPosition)
  {
    List<Collider> validTargets = new();

    Collider[] colliders = Physics.OverlapSphere(parPosition, parRadius, ~_ignoreMask);

    foreach (var collider in colliders)
    {
      if (collider == null)
        continue;

      if (!collider.TryGetComponent(out Combat parCombat))
        continue;

      if (collider.GetComponent<PlayerControl>() || collider.GetComponent<SnakePart>())
        continue;

      validTargets.Add(collider);
    }

    GameManager.Instance.BoomManager.BoomWithTargets(parPosition, parRadius, parForce, parDamage, 3, validTargets.ToArray());
  }

  public void CollectDiamond(int parEnergyValue, DropItemBase parDropItemBase)
  {
    CurrentDiamond++;
    CurrentEnergy += parEnergyValue;

    if (CurrentEnergy >= Goal)
    {
      currentLevel++;
      CurrentEnergy = 0;

      CreateFollower();
    }

    RemoveDropItem(parDropItemBase);
  }
  
  private void CreateFollower()
  {
    StartCoroutine(TrySpawnFollower());
  }

  public void CollectAllDiamonds()
  {
    StartCoroutine(MagnetDiamond());
  }

  private IEnumerator TrySpawnFollower()
  {
    WaitForSeconds waitForSeconds = new(1f);

    int maxAttempts = 10;
    int nearbyAttempts = 8;
    float checkRadius = 1f;
    bool found = false;

    while (!found)
    {
      for (int i = 0; i < maxAttempts; i++)
      {
        Transform spawnPoint = enemySpawner.SpawnPoints[Random.Range(0, enemySpawner.SpawnPoints.Count)];

        if (!Physics.CheckSphere(spawnPoint.position, checkRadius, _mask))
        {
          Instantiate(_snakePartPrefab, spawnPoint.position, Quaternion.identity);
          found = true;
          break;
        }

        for (int j = 0; j < nearbyAttempts; j++)
        {
          Vector3 offset = Random.onUnitSphere;
          offset.y = 0;
          offset = offset.normalized * 2f;

          Vector3 nearbyPosition = spawnPoint.position + offset;

          if (!Physics.CheckSphere(nearbyPosition, checkRadius, _mask))
          {
            Instantiate(_snakePartPrefab, nearbyPosition, Quaternion.identity);
            found = true;
            break;
          }
        }
      }

      if (!found)
        yield return waitForSeconds;
    }
  }

  private IEnumerator MagnetDiamond()
  {
    Transform player = GameManager.Instance.LevelManager.Player.transform;

    float timeToReach = 0.05f;

    while (true)
    {
      bool anyMoving = false;

      foreach (var dropItem in listDropItems)
      {
        if (dropItem == null)
          continue;

        DiamondPickup diamond = dropItem as DiamondPickup;
        if (diamond == null)
          continue;

        float distance = Vector3.Distance(diamond.transform.position, player.position);
        float speed = distance / timeToReach;

        diamond.transform.position += (player.position - diamond.transform.position).normalized * speed * Time.unscaledDeltaTime;

        anyMoving = true;
      }

      if (!anyMoving)
        break;

      yield return null;
    }
  }
}