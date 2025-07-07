using UnityEngine;
//using GamePush;

public class GamePushManager : MonoBehaviour
{
  public bool IsInit { get; private set; }

  public bool IsAdsRunning { get; private set; }

  private void OnEnable()
  {
    //GP_Init.OnReady += GP_Init_OnReady;
  }

  private void GP_Init_OnReady()
  {
    IsInit = true;
  }

  public void SetAdsRunning(bool parValue)
  {
    IsAdsRunning = parValue;
  }
}