using UnityEngine;
using UnityEngine.SceneManagement;
using GamePush;
using System;

public class LevelManager : MonoBehaviour
{
  public enum gameModes
  {
	rescue,
	single
  }

  public enum game3Ctypes
  {
	topDown,
	fps
  }

  public enum gameStates
  {
	notStart,
	playing,
	defeat,
	success
  }

  public struct PauseContext
  {
	public bool paused;
  }

  public delegate void PauseEventHandler(PauseContext pauseContext);

  [HideInInspector]
  public gameModes gameMode;

  [HideInInspector]
  public game3Ctypes game3CType;

  [HideInInspector]
  public LevelInfo levelInfo;

  private int levelIndex;

  private int totalBroCount;

  private int savedBroCount;

  private int unsavedBroCount;

  private int teamBroCount;

  private int deadBroCount;

  private int enemyKillCount;

  private int targetCount;

  private gameStates gameState;

  private bool paused;

  [SerializeField]
  private PlayerControl playerPfb;

  private PlayerControl player;

  [SerializeField]
  private Pointer pointerPfb;

  private Pointer pointer;

  private bool readyToLoad;

  private int toLoadIndex;

  private bool countStarted;

  public static LevelManager instance
  {
	get
	{
	  if ((bool)GameManager.Instance)
	  {
		return GameManager.Instance.LevelManager;
	  }
	  return null;
	}
  }

  public int LevelIndex => levelIndex;

  public int TotalBroCount => totalBroCount;

  public int SavedBroCount => savedBroCount;

  public int UnsavedBroCount => unsavedBroCount;

  public int TeamBroCount => teamBroCount;

  public int DeadBroCount => deadBroCount;

  public int EnemyKillCount => enemyKillCount;

  public bool SurvivalMode { get; private set; }
  public bool DontSaveLevelNumber { get; private set; }
  
  public gameStates GameState => gameState;

  public static bool Paused
  {
	get
	{
	  if ((bool)instance)
	  {
		return instance.paused;
	  }
	  return false;
	}
	set
	{
	  if ((bool)instance && instance.paused != value)
	  {
		instance.paused = value;
		instance.onPauseChanged?.Invoke(new PauseContext
		{
		  paused = value
		});
	  }
	}
  }

  public PlayerControl Player => player;

  public Transform Pointer
  {
	get
	{
	  if (!(pointer == null))
	  {
		return pointer.transform;
	  }
	  return null;
	}
  }

  public event Action<bool> OnStartSurvivalMode;
  public event Action<SurvivalManager> OnInitializeSurivalMode;

  public event PauseEventHandler onPauseChanged;

  public static void Pause()
  {
	Paused = true;
  }

  public static void Resume()
  {
	Paused = false;
  }

  public void SetLevelIndex(int _index)
  {
	levelIndex = _index;
  }

  private void Awake()
  {
	levelIndex = 0;
	LoadLevel(0);
  }

  private void Start()
  {
  }

  private void Update()
  {
	Cursor.lockState = CursorLockMode.None;
	if (paused)
	{
	  Cursor.visible = true;
	}
	if (readyToLoad)
	{
	  return;
	}
	if (gameState == gameStates.defeat)
	{
	  if (UnityEngine.Input.GetKeyDown(KeyCode.R))
	  {
		RestartLevel();

      }
	}
	else if (gameState == gameStates.success)
	{
	  if (UnityEngine.Input.GetKeyDown(KeyCode.R))
	  {
		NextLevel();
	  }
	}
	else if (!paused)
	{
	  Cursor.visible = false;
	  if (game3CType == game3Ctypes.fps)
	  {
		Cursor.lockState = CursorLockMode.Locked;
	  }
	}
  }

  public void ResumeGame()
  {
    if (gameState == gameStates.defeat)
    {
      gameState = gameStates.playing;
      GameManager.Instance.CameraManager.RenderCamera.FadeIn();
      pointer.gameObject.SetActive(InputManager.Instance.IsPC);
      GameManager.Instance.TimeScaleManager.ResetTimeScales();
    }
  }

  public void RestartLevel()
  {
    TryLoadLevel(levelIndex);
  }
  
  public void NextLevel()
  {
    if (levelInfo.hasNextLevel)
    {
      TryLoadLevel(levelIndex + 1);
    }
    else
    {
      TryLoadLevel(0);
    }
  }

  public void StartSurvivalZone()
  {
    if (!GameManager.Instance.GamePushManager.IsInit || GameManager.Instance.GamePushManager.IsAdsRunning)
      return;

    GP_Ads.ShowRewarded("SURVIVAL", StartAdsSurvivalZone, OnRewardedStart);
  }

  private void OnRewardedStart()
  {
    GameManager.Instance.GamePushManager.SetAdsRunning(true);
  }

  private void StartAdsSurvivalZone(string parValue)
  {
    switch (parValue)
    {
      case "SURVIVAL":
		NoSaveLevelNumber(true);
        SetSurvivalMode(true);
        GameManager.Instance.GamePushManager.SetAdsRunning(false);
        TryLoadLevel(999, false);
        break;
    }
  }

  public void SetSurvivalMode(bool parValue)
  {
	SurvivalMode = parValue;

	OnStartSurvivalMode?.Invoke(parValue);
  }

  public void NoSaveLevelNumber(bool parValue)
  {
	DontSaveLevelNumber = parValue;
  }

  public void InitializeSurvivalMode(SurvivalManager parSurvivalManager)
  {
	OnInitializeSurivalMode?.Invoke(parSurvivalManager);
  }

  public void StartLevel(Vector3 _startPoint, int _targetCount)
  {
	if (gameMode == gameModes.rescue)
	{
	  targetCount = _targetCount;
	}
	readyToLoad = false;
	InitCount();
	gameState = gameStates.playing;
	player = Instantiate(playerPfb, _startPoint, Quaternion.identity);
    pointer = Instantiate(pointerPfb);
    GameManager.Instance.CameraManager.Init(player, pointer.transform);
	pointer.virtualCamera = GameManager.Instance.CameraManager.TopDownCameraArm.VirtualCamera;
	GameManager.Instance.TimeScaleManager.ResetTimeScales();
	enemyKillCount = 0;
	UpdateUICount();
	Paused = false;

    if (GameManager.Instance.GamePushManager.IsInit)
      GP_Ads.ShowFullscreen();
  }

  public void Defeat(DefeatType _type)
  {
	if (gameState == gameStates.success || gameState == gameStates.defeat)
	{
	  return;
	}
	pointer.gameObject.SetActive(value: false);
	AudioManager.PlaySFX("Game_Fail");
	if (readyToLoad || gameState != gameStates.playing)
	{
	  return;
	}
	GameManager.Instance.TimeScaleManager.Dead();
    GameManager.Instance.CameraManager.TopDownCameraArm.Over();
	gameState = gameStates.defeat;

    if (GameManager.Instance.GamePushManager.IsInit)
      GP_Ads.ShowFullscreen();

    bool flag = LocalizationSync.IsChinese();
	switch (_type)
	{
	  case DefeatType.brosDead:
		if (flag)
		{
		  switch (UnityEngine.Random.Range(0, 4))
		  {
			case 0:
			  GameManager.Instance.UIManager.Defeat("你的兄弟挂的太多了。");
			  break;
			case 1:
			  GameManager.Instance.UIManager.Defeat("剩下的不够完成指标了。");
			  break;
			case 2:
			  GameManager.Instance.UIManager.Defeat("注意你的尾巴。");
			  break;
			case 3:
			  GameManager.Instance.UIManager.Defeat("他们去哪了？");
			  break;
		  }
		}
		else
		{
		  GameManager.Instance.UIManager.Defeat("Too many bros have died.");
		}
		break;
	  case DefeatType.died:
		if (flag)
		{
		  switch (UnityEngine.Random.Range(0, 4))
		  {
			case 0:
			  GameManager.Instance.UIManager.Defeat("菜。");
			  break;
			case 1:
			  GameManager.Instance.UIManager.Defeat("你被干掉了。");
			  break;
			case 2:
			  GameManager.Instance.UIManager.Defeat("你挂了。");
			  break;
			case 3:
			  GameManager.Instance.UIManager.Defeat("Snake!!!!");
			  break;
		  }
		}
		else
		{
		  GameManager.Instance.UIManager.Defeat("You died.");

		  if (SurvivalMode)
            GameManager.Instance.UIManager.Revival(true);
        }
		break;
	  case DefeatType.timeUp:
		if (flag)
		{
		  switch (UnityEngine.Random.Range(0, 4))
		  {
			case 0:
			  GameManager.Instance.UIManager.Defeat("时不我待");
			  break;
			case 1:
			  GameManager.Instance.UIManager.Defeat("时不待我");
			  break;
			case 2:
			  GameManager.Instance.UIManager.Defeat("时间不等人。");
			  break;
			case 3:
			  GameManager.Instance.UIManager.Defeat("下次动作快点。");
			  break;
		  }
		}
		else
		{
		  GameManager.Instance.UIManager.Defeat("Time's up.");
		}
		break;
	}
	DataManager.Save();
  }

  public void Success()
  {
	if (!readyToLoad && gameState == gameStates.playing)
	{
	  GameManager.Instance.CameraManager.TopDownCameraArm.Over();
	  gameState = gameStates.success;
	  AudioManager.PlaySFX("Game_Success");
	  GameManager.Instance.UIManager.Success();
	  if (enemyKillCount == 0 && levelIndex != 7)
	  {
		DataManager.SetAchievement("peaceElite");
	  }
	  DataManager.Save();
	  switch (GameManager.Instance.currentLevel)
	  {
		case 1:
		  DataManager.CountLevel1Finished();
		  break;
		case 2:
		  DataManager.CountLevel2Finished();
		  break;
		case 3:
		  DataManager.CountLevel3Finished();
		  break;
		case 4:
		  DataManager.CountLevel4Finished();
		  break;
		case 5:
		  DataManager.CountLevel5Finished();
		  break;
	  }
	  DataManager.LevelSuccessContext context = default(DataManager.LevelSuccessContext);
	  context.levelIndex = GameManager.Instance.currentLevel;
	  context.teamCount = TeamBroCount;
	  DataManager.CountLevelSuccess(context);
	}
  }

  public void InitCount()
  {
	totalBroCount = 0;
	savedBroCount = 0;
	unsavedBroCount = 0;
	teamBroCount = 0;
	deadBroCount = 0;
	UpdateUICount();
  }

  public void CountABro()
  {
	countStarted = true;
	totalBroCount++;
	savedBroCount = savedBroCount;
	unsavedBroCount++;
	teamBroCount = teamBroCount;
	deadBroCount = deadBroCount;
	UpdateUICount();
  }

  public void SaveABro()
  {
	totalBroCount = totalBroCount;
	savedBroCount++;
	unsavedBroCount--;
	teamBroCount++;
	deadBroCount = deadBroCount;
	UpdateUICount();
	DataManager.SaveBroContext context = default(DataManager.SaveBroContext);
	context.teamCount = TeamBroCount;
	DataManager.SavedABro(context);
  }

  public void KillABro(bool saved)
  {
	totalBroCount = totalBroCount;
	savedBroCount = savedBroCount;
	if (saved)
	{
	  teamBroCount--;
	}
	else
	{
	  unsavedBroCount--;
	}
	deadBroCount++;
	if (teamBroCount + unsavedBroCount < targetCount)
	{
	  if (!SurvivalMode)
		Defeat(DefeatType.brosDead);
	}
	UpdateUICount();
  }

  public void KillAEnemy()
  {
	enemyKillCount++;
  }

  private void UpdateUICount()
  {
	GameManager.Instance.UIManager.UpdateCount(teamBroCount, unsavedBroCount, targetCount);
  }

  public void TryLoadLevel(int _levelIndex, bool delay = false)
  {
	if (!readyToLoad)
	{
	  readyToLoad = true;
	  GameManager.Instance.CameraManager.RenderCamera.FadeOut();
	  toLoadIndex = _levelIndex;
	  Invoke("AutoLoadLevel", 1.5f * Time.timeScale);
	}
  }

  public bool CheckCanSuccess()
  {
	if (teamBroCount >= targetCount)
	{
	  return true;
	}
	return false;
  }

  private void AutoLoadLevel()
  {
	LoadLevel(toLoadIndex);
  }

  private void LoadLevel(int _levelIndex)
  {
	levelIndex = _levelIndex;
	SceneManager.LoadScene("level" + _levelIndex.ToString(), LoadSceneMode.Single);
  }

  private void FadeIn()
  {
	if (GameManager.Instance.CameraManager.TopDownCameraArm != null)
	{
	  GameManager.Instance.CameraManager.RenderCamera.FadeIn();
	}
  }

  public static void ReloadCurrentLevel()
  {
	if (instance != null)
	{
	  instance.TryLoadLevel(instance.levelIndex);
	}
  }
}