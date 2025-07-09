using UnityEngine;

public class FollowerIndicator : MonoBehaviour
{
  [SerializeField, Min(0)] private float _lerpSpeed = 10f;

  [SerializeField, Min(0)] private float _detectionRadius = 500f;

  [SerializeField] private LayerMask _followerMask;

  [SerializeField] private GameObject _triObject;

  private Transform nearestFollow;
  private Collider[] hits = new Collider[5];

  private Transform parent;
  private Vector3 offset;

  private LevelManager levelManager;

  private void Awake()
  {
    levelManager = GameManager.Instance.LevelManager;
  }

  private void Start()
  {
    gameObject.SetActive(levelManager != null && levelManager.SurvivalMode);

    parent = transform.parent;
    offset = transform.localPosition;
    transform.SetParent(null);
  }

  private void Update()
  {
    if (levelManager == null || !levelManager.SurvivalMode)
      return;

    FindNearestFollow();

    transform.position = parent.TransformPoint(offset);

    if (nearestFollow != null)
    {
      Vector3 direction = nearestFollow.position - transform.position;
      direction.y = 0;

      if (direction.sqrMagnitude > 0.001f)
      {
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, _lerpSpeed * Time.unscaledDeltaTime);
      }
    }
    else
    {
      transform.rotation = Quaternion.Lerp(transform.rotation, parent.rotation, _lerpSpeed * Time.unscaledDeltaTime);
    }
  }

  private void FindNearestFollow()
  {
    int count = Physics.OverlapSphereNonAlloc(transform.position, _detectionRadius, hits, _followerMask);

    _triObject.SetActive(count > 0);

    if (count <= 0)
    {
      nearestFollow = null;
      return;
    }

    float distance = Mathf.Infinity;
    nearestFollow = null;

    for (int i = 0; i < count; i++)
    {
      float distanceSqr = (hits[i].transform.position - parent.position).sqrMagnitude;

      if (distanceSqr < distance)
      {
        distance = distanceSqr;
        nearestFollow = hits[i].transform;
      }
    }
  }
}