using Com.LuisPedroFonseca.ProCamera2D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombPickup : DropItemBase
{
  [SerializeField, Min(0)] private int _damage = 9999;
  [SerializeField, Min(0)] private float _boomRadius = 6f;
  [SerializeField, Min(0)] private float _boomForce = 500f;

  public override void OnPickup(GameObject parPicker)
  {
    SurvivalManager.Instance.KillEnemiesRadius(_boomRadius, _boomForce, _damage, transform.position);

    Destroy(gameObject);
  }
}