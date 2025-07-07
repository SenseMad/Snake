using GamePush;
using UnityEngine;

public class SurvivalZone : FinishZone
{
  public bool isOneTimeUnlockAds;

  protected override void Start()
  {
    if (DataManager.NumberCompletedLevels() > 7)
      return;

    base.Start();
  }

  protected override void Update()
  {
    if (GameManager.Instance.GamePushManager.IsAdsRunning)
      return;

    if (!isSlecter)
    {
      if (!canSuccess && GameManager.Instance.LevelManager.CheckCanSuccess())
      {
        canSuccess = true;
        finshObj.SetActive(value: true);
        unfinshObj.SetActive(value: false);
      }
      else if (canSuccess && !GameManager.Instance.LevelManager.CheckCanSuccess())
      {
        canSuccess = false;
        finshObj.SetActive(value: false);
        unfinshObj.SetActive(value: true);
      }
      if (canSuccess && Vector3.Distance(GameManager.Instance.LevelManager.Player.transform.position, base.transform.position) < radius)
      {
        GameManager.Instance.LevelManager.Success();
      }
    }
    else if (!locked && Vector3.Distance(GameManager.Instance.LevelManager.Player.transform.position, base.transform.position) < radius)
    {
      GameManager.Instance.LevelManager.gameMode = gameMode;
      GameManager.Instance.LevelManager.game3CType = game3CType;
      GameManager.Instance.LevelManager.SetSurvivalMode(true);
      GameManager.Instance.LevelManager.TryLoadLevel(selectIndex);
    }

    if (isOneTimeUnlockAds && locked)
    {
      if (Vector3.Distance(GameManager.Instance.LevelManager.Player.transform.position, transform.position) < radius)
      {
        GameManager.Instance.UIManager.AccessToLevelAds(true);

        if (Input.GetKeyDown(KeyCode.R))
          GP_Ads.ShowRewarded("SURVIVAL", OnRewardedReward, OnRewardedStart);
      }
      else
      {
        GameManager.Instance.UIManager.AccessToLevelAds(false);
      }
    }
  }

  private void OnRewardedStart()
  {
    GameManager.Instance.GamePushManager.SetAdsRunning(true);
  }

  private void OnRewardedReward(string parValue)
  {
    switch (parValue)
    {
      case "SURVIVAL":
        GameManager.Instance.LevelManager.SetSurvivalMode(true);
        GameManager.Instance.GamePushManager.SetAdsRunning(false);
        GameManager.Instance.LevelManager.TryLoadLevel(999, false);
        break;
    }
  }
}