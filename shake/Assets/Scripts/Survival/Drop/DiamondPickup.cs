using UnityEngine;

public class DiamondPickup : DropItemBase
{
  [SerializeField, Min(0)] private int _energyValue = 10;

  public override void OnPickup(GameObject parPicker)
  {
    SurvivalManager survivalManager = SurvivalManager.Instance;
    survivalManager.CollectDiamond(_energyValue, this);

    Destroy(gameObject);
  }
}