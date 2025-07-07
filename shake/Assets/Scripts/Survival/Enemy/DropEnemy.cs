using UnityEngine;

public class DropEnemy : MonoBehaviour
{
  [Header("Prefabs")]
  [SerializeField] private DropItemBase _diamondPrefab;
  [SerializeField] private DropItemBase _heartPrefab;
  [SerializeField] private DropItemBase _bombPrefab;
  [SerializeField] private DropItemBase _magnetPrefab;

  [Header("Drop chances (0 to 1)")]
  [SerializeField, Min(0)] private float _heartChance = 0.1f;
  [SerializeField, Min(0)] private float _bombChance = 0.05f;
  [SerializeField, Min(0)] private float _magnetChance = 0.03f;

  private Combat enemy;

  private void Awake()
  {
    enemy = GetComponent<Combat>();
  }

  private void OnEnable()
  {
    enemy.OnDied += Enemy_OnDied;
  }

  private void OnDestroy()
  {
    enemy.OnDied -= Enemy_OnDied;
  }

  public void Drop(Vector3 parPosition)
  {
    SurvivalManager survivalManager = SurvivalManager.Instance;

    DropItemBase dropItemBase = Instantiate(_diamondPrefab, parPosition, Quaternion.identity);

    survivalManager.AddDropItem(dropItemBase);

    DropItemBase dropItemBaseChance = null;

    float roll = Random.value;

    if (roll < _magnetChance)
      dropItemBaseChance = Instantiate(_magnetPrefab, parPosition + Vector3.right * 0.5f, Quaternion.identity);
    else if (roll < _magnetChance + _bombChance)
      dropItemBaseChance = Instantiate(_bombPrefab, parPosition + Vector3.right * 0.5f, Quaternion.identity);
    else if (roll < _magnetChance + _bombChance + _heartChance)
      dropItemBaseChance = Instantiate(_heartPrefab, parPosition + Vector3.right * 0.5f, Quaternion.identity);

    if (dropItemBaseChance != null)
      survivalManager.AddDropItem(dropItemBaseChance);
  }

  private void Enemy_OnDied(Combat parEnemy)
  {
    Drop(parEnemy.transform.position);
  }
}