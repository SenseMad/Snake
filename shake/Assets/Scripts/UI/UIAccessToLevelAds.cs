using UnityEngine;
using UnityEngine.UI;

public class UIAccessToLevelAds : MonoBehaviour
{
  [SerializeField] private CanvasGroup _canvasGroup;

  [SerializeField] private Button _button;

  [SerializeField] private GameObject _darkGlowObject;

  [SerializeField] private Text _returnText;

  private LevelManager levelManager;
  private InputManager inputManager;

  private void OnDestroy()
  {
    _button.onClick.RemoveListener(levelManager.StartSurvivalZone);
  }

  public void Initialize()
  {
    levelManager = GameManager.Instance.LevelManager;
    inputManager = InputManager.Instance;

    _button.onClick.AddListener(levelManager.StartSurvivalZone);

    _canvasGroup.gameObject.SetActive(false);
  }

  public void SetActive(bool parValue)
  {
    bool isMobile = inputManager.CurrentInputType == InputType.Mobile;

    _button.gameObject.SetActive(isMobile);
    _returnText.gameObject.SetActive(!isMobile);
    _darkGlowObject.SetActive(!isMobile);

    _canvasGroup.gameObject.SetActive(parValue);
  }

  public void SetAlpha(float parAlpha)
  {
    _canvasGroup.alpha = parAlpha;
  }
}