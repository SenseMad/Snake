using UnityEngine;
using GamePush;
using System;

public class GamePushManager : MonoBehaviour
{
  private static GamePushManager instance;

  public static GamePushManager Instance => instance;

  public bool IsInit { get; private set; }

  public bool IsAdsRunning { get; private set; }

  public event Action OnGamePushInit;

  private void Awake()
  {
    instance = this;
  }

  private void OnEnable()
  {
    GP_Init.OnReady += GP_Init_OnReady;
  }

  private void GP_Init_OnReady()
  {
    IsInit = true;

    OnGamePushInit?.Invoke();
  }

  public void SetAdsRunning(bool parValue)
  {
    IsAdsRunning = parValue;
  }
}