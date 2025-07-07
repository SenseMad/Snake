using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
  [SerializeField]
  private Canvas uiCanvas;

  [Header("References")]
  [SerializeField] private UIFailedPanel _failedPanel;
  [SerializeField] private UISuccessPanel _successPanel;
  [SerializeField] private UIAccessToLevelAds _accessToLevelAds;

  [Space]
  [SerializeField]
  private Image healthBarImage;

  private Material healthBarMat;

  [SerializeField]
  private Text remainCountText;

  [SerializeField]
  private Text targetCountText;

  public Color redColor;

  public Color greenColor;

  [SerializeField]
  private CanvasGroup accessToLevelAdsCanvasGroup;

  [SerializeField]
  private AnimationCurve overShowCurve;

  [SerializeField]
  private float overShowTime;

  [SerializeField]
  private GameObject hudPanel;

  [SerializeField]
  private GameObject mobileUI;

  private float overShowTimer;

  private Color tempColor;

  private bool overIsSuccess;

  private bool over;

  private void Awake()
  {
	healthBarMat = healthBarImage.material;
  }

  public void Init(Camera _camera)
  {
	_failedPanel.Initialize();
	_successPanel.Initialize();
	_accessToLevelAds.Initialize();

    uiCanvas.worldCamera = _camera;
	uiCanvas.planeDistance = 1f;
	hudPanel.SetActive(value: true);
	over = false;
	overShowTimer = 0f;
  }

  public void UpdateCount(int _teamCount, int _remainCount, int _targetCount)
  {
	remainCountText.text = _remainCount.ToString();
	targetCountText.text = _teamCount.ToString() + "/" + _targetCount.ToString();
	if (_teamCount >= _targetCount)
	{
	  targetCountText.color = greenColor;
	}
	else
	{
	  targetCountText.color = redColor;
	}
  }

  private void Update()
  {
	if (GameManager.Instance.LevelManager.Player != null)
	{
	  healthBarMat.SetFloat("_CutValue", GameManager.Instance.LevelManager.Player.Combat.HealthPercent);
	}
	if (over && overShowTimer < overShowTime)
	{
	  overShowTimer += Time.unscaledDeltaTime;
	  overShowTimer = Mathf.Min(overShowTimer, overShowTime);

	  float alpha = overShowCurve.Evaluate(overShowTimer / overShowTime);
      if (!overIsSuccess)
        _failedPanel.SetAlpha(alpha);
      else
        _successPanel.SetAlpha(alpha);
    }
  }

  public void RestoreHUD()
  {
    over = false;
    overShowTimer = 0f;

	_failedPanel.Hide();
	_successPanel.Hide();

    hudPanel.SetActive(true);

    if (InputManager.Instance.CurrentInputType == InputType.Mobile)
      mobileUI.SetActive(true);
  }

  public void Defeat(string _defeatString)
  {
    overIsSuccess = false;
    over = true;

    _failedPanel.Show(_defeatString);
    hudPanel.SetActive(false);

	if (InputManager.Instance.CurrentInputType == InputType.Mobile)
	  mobileUI.SetActive(false);
  }

  public void Success()
  {
	overIsSuccess = true;
    over = true;

	_successPanel.Show();
    hudPanel.SetActive(false);

    if (InputManager.Instance.CurrentInputType == InputType.Mobile)
      mobileUI.SetActive(false);
  }

  public void AccessToLevelAds(bool parValue)
  {
	_accessToLevelAds.SetActive(parValue);
  }

  public void Revival(bool parValue)
  {
	_failedPanel.SetRevivalButton(parValue);

    if (InputManager.Instance.CurrentInputType == InputType.Mobile)
      mobileUI.SetActive(!parValue);
  }

  public void ChangeTextAlpha(Text _text, float alpha)
  {
	tempColor = _text.color;
	tempColor.a = alpha;
	_text.color = tempColor;
  }
}