using GamePush;
using UnityEngine;

public class Player : MonoBehaviour
{
  private Combat combat;

  private void Awake()
  {
    combat = GetComponent<Combat>();
  }

  private void OnEnable()
  {
    combat.OnDied += Combat_OnDied;
  }

  private void OnDestroy()
  {
    combat.OnDied -= Combat_OnDied;
  }

  private void Combat_OnDied(Combat obj)
  {
    if (GameManager.Instance.GamePushManager.IsInit)
      GP_Ads.ShowFullscreen();
  }
}