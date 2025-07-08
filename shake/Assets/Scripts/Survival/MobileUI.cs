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
  [SerializeField] private Button _pauseButton;

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
    inputManager.OnDeviceSelection += InputManager_OnDeviceSelection;

    _aimButton.onClick.AddListener(inputManager.OnAimButtonPressed);

    _jumpButton.onClick.AddListener(inputManager.OnJumpButtonPressed);

    _pauseButton.onClick.AddListener(LevelManager.Pause);
  }

  private void OnDisable()
  {
    inputManager.OnDeviceSelection -= InputManager_OnDeviceSelection;

    _aimButton.onClick.RemoveListener(inputManager.OnAimButtonPressed);

    _jumpButton.onClick.RemoveListener(inputManager.OnJumpButtonPressed);

    _pauseButton.onClick.RemoveListener(LevelManager.Pause);
  }

  private void Update()
  {
    if (_moveJoystick != null)
      inputManager.SetMoveInput(_moveJoystick.Direction);

    if (_lookJoystick != null)
      inputManager.SetLookInput(_lookJoystick.Direction);
  }

  private void InputManager_OnDeviceSelection(InputType parInputType)
  {
    _mobileUI.SetActive(parInputType == InputType.Mobile);
  }

  private void AddEventTrigger(EventTrigger trigger, EventTriggerType type, UnityAction<BaseEventData> callback)
  {
    var entry = new EventTrigger.Entry { eventID = type };
    entry.callback.AddListener(callback);
    trigger.triggers.Add(entry);
  }
}