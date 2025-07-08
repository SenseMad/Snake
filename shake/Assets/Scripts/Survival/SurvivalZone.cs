using GamePush;
using UnityEngine;

public class SurvivalZone : FinishZone
{
  public bool isOneTimeUnlockAds;

  private GameManager gameManager;

  private void Awake()
  {
    gameManager = GameManager.Instance;
  }

  protected override void Start()
  {
    if (DataManager.NumberCompletedLevels() > 6)
      return;

    base.Start();
  }

  protected override void Update()
  {
    if (gameManager == null)
      return;

    if (gameManager.GamePushManager.IsAdsRunning)
      return;

    if (!isSlecter)
    {
      if (!canSuccess && gameManager.LevelManager.CheckCanSuccess())
      {
        canSuccess = true;
        finshObj.SetActive(value: true);
        unfinshObj.SetActive(value: false);
      }
      else if (canSuccess && !gameManager.LevelManager.CheckCanSuccess())
      {
        canSuccess = false;
        finshObj.SetActive(value: false);
        unfinshObj.SetActive(value: true);
      }
      if (canSuccess && Vector3.Distance(gameManager.LevelManager.Player.transform.position, base.transform.position) < radius)
      {
        gameManager.LevelManager.Success();
      }
    }
    else if (!locked && Vector3.Distance(gameManager.LevelManager.Player.transform.position, base.transform.position) < radius)
    {
      gameManager.LevelManager.NoSaveLevelNumber(false);
      gameManager.LevelManager.gameMode = gameMode;
      gameManager.LevelManager.game3CType = game3CType;
      gameManager.LevelManager.SetSurvivalMode(true);
      gameManager.LevelManager.TryLoadLevel(selectIndex);
    }

    if (isOneTimeUnlockAds && locked)
    {
      if (Vector3.Distance(gameManager.LevelManager.Player.transform.position, transform.position) < radius)
      {
        gameManager.UIManager.AccessToLevelAds(true);

        if (Input.GetKeyDown(KeyCode.R))
          gameManager.LevelManager.StartSurvivalZone();
      }
      else
      {
        gameManager.UIManager.AccessToLevelAds(false);
      }
    }
  }
}