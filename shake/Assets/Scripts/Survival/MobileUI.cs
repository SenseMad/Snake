using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

public class MobileUI : MonoBehaviour
{
  [Header("Mobile UI")]
  [SerializeField] private GameObject _mobileUI;
  [SerializeField] private Joystick _moveJoystick;
  [SerializeField] private Joystick _lookJoystick;
  [SerializeField] private Button _shootButton;
  [SerializeField] private Button _aimButton;
  [SerializeField] private Button _jumpButton;
  [SerializeField] private Button _dashButton;

  private InputManager inputManager;

  private void Awake()
  {
    inputManager = InputManager.Instance;
  }

  private void Start()
  {
    if (!_shootButton.gameObject.TryGetComponent<EventTrigger>(out var triggerShootButton))
      triggerShootButton = _shootButton.gameObject.AddComponent<EventTrigger>();

    triggerShootButton.triggers.Clear();
    AddEventTrigger(triggerShootButton, EventTriggerType.PointerDown, _ => inputManager.OnShootButtonDown());
    AddEventTrigger(triggerShootButton, EventTriggerType.PointerUp, _ => inputManager.OnShootButtonUp());
  }

  private void OnEnable()
  {
    SetupUI();

    _aimButton.onClick.AddListener(inputManager.OnAimButtonPressed);

    _jumpButton.onClick.AddListener(inputManager.OnJumpButtonPressed);
  }

  private void OnDisable()
  {
    _aimButton.onClick.RemoveListener(inputManager.OnAimButtonPressed);

    _jumpButton.onClick.RemoveListener(inputManager.OnJumpButtonPressed);
  }

  private void Update()
  {
    if (_moveJoystick != null)
      inputManager.SetMoveInput(_moveJoystick.Direction);

    if (_lookJoystick != null)
      inputManager.SetLookInput(_lookJoystick.Direction);
  }

  private void SetupUI()
  {
    _mobileUI?.SetActive(inputManager.CurrentInputType == InputType.Mobile);
  }

  private void AddEventTrigger(EventTrigger trigger, EventTriggerType type, UnityAction<BaseEventData> callback)
  {
    var entry = new EventTrigger.Entry { eventID = type };
    entry.callback.AddListener(callback);
    trigger.triggers.Add(entry);
  }
}