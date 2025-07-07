using UnityEngine;

public class MagnetPickup : DropItemBase
{
  public override void OnPickup(GameObject parPicker)
  {
    SurvivalManager.Instance.CollectAllDiamonds();

    Destroy(gameObject);
  }
}