using UnityEngine;
using UnityEngine.UI;

public class UIFailedPanel : MonoBehaviour
{
  [SerializeField] private CanvasGroup _canvasGroup;

  [SerializeField] private GameObject _darkGlowObject;
  [SerializeField] private GameObject _revivalDarkGlowObject;

  [Header("Buttons")]
  [SerializeField] private Button _restartButton;
  [SerializeField] private Button _revivalButton;

  [Header("Texts")]
  [SerializeField] private Text _defeatText;
  [SerializeField] private Text _returnText;
  [SerializeField] private Text _revivalText;

  private LevelManager levelManager;
  private InputManager inputManager;

  private void OnDestroy()
  {
    _restartButton.onClick.RemoveListener(levelManager.RestartLevel);
    _revivalButton.onClick.RemoveListener(levelManager.Player.Combat.Revival);
  }

  public void Initialize()
  {
    levelManager = GameManager.Instance.LevelManager;
    inputManager = InputManager.Instance;

    _restartButton.onClick.AddListener(levelManager.RestartLevel);
    _revivalButton.onClick.AddListener(levelManager.Player.Combat.Revival);

    _canvasGroup.alpha = 0f;
    _canvasGroup.gameObject.SetActive(false);
  }

  public void Show(string parMessage)
  {
    bool isMobile = inputManager.CurrentInputType == InputType.Mobile;

    _restartButton.gameObject.SetActive(isMobile);
    _revivalButton.gameObject.SetActive(isMobile && (levelManager.Player.Combat.IsDead || levelManager.SurvivalMode));

    _returnText.gameObject.SetActive(!isMobile);
    _revivalText.gameObject.SetActive(!isMobile && (levelManager.Player.Combat.IsDead || levelManager.SurvivalMode));

    _darkGlowObject.SetActive(!isMobile);
    _revivalDarkGlowObject.SetActive(!isMobile && (levelManager.Player.Combat.IsDead || levelManager.SurvivalMode));

    _defeatText.text = parMessage;
    _canvasGroup.gameObject.SetActive(true);
  }

  public void SetRevivalButton(bool parValue)
  {
    bool isMobile = inputManager.CurrentInputType == InputType.Mobile;

    _revivalButton.gameObject.SetActive(parValue && isMobile && (levelManager.Player.Combat.IsDead || levelManager.SurvivalMode));
  }

  public void Hide()
  {
    _canvasGroup.alpha = 0f;
    _canvasGroup.gameObject.SetActive(false);
  }

  public void SetAlpha(float parAlpha)
  {
    _canvasGroup.alpha = parAlpha;
  }
}