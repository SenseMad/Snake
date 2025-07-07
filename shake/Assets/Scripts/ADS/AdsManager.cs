using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamePush;

public class AdsManager : MonoBehaviour
{
  public void ShowFullscreen()
  {
    if (!GameManager.Instance.GamePushManager.IsInit)
      return;

    GP_Ads.ShowFullscreen();
  }
}