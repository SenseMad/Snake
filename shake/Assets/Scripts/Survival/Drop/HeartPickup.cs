using UnityEngine;

public class HeartPickup : DropItemBase
{
  public override void OnPickup(GameObject parPicker)
  {
    GameManager.Instance.LevelManager.Player.Combat.RestoreHealth();

    Destroy(gameObject);
  }
}