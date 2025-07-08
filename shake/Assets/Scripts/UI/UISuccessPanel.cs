using UnityEngine;
using UnityEngine.UI;

public class UISuccessPanel : MonoBehaviour
{
  [SerializeField] private CanvasGroup _canvasGroup;

  [SerializeField] private Button _button;

  [SerializeField] private GameObject _darkGlowObject;

  [SerializeField] private Text _returnText;

  private LevelManager levelManager;
  private InputManager inputManager;

  private void OnDestroy()
  {
    _button.onClick.RemoveListener(levelManager.NextLevel);
  }

  public void Initialize()
  {
    levelManager = GameManager.Instance.LevelManager;
    inputManager = InputManager.Instance;

    _button.onClick.AddListener(levelManager.NextLevel);

    _canvasGroup.alpha = 0f;
    _canvasGroup.gameObject.SetActive(false);
  }

  public void Show()
  {
    bool isMobile = inputManager.CurrentInputType == InputType.Mobile;

    _button.gameObject.SetActive(isMobile);
    _returnText.gameObject.SetActive(!isMobile);
    _darkGlowObject.SetActive(!isMobile);

    _canvasGroup.gameObject.SetActive(true);
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