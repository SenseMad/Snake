//using GamePush;
using UnityEngine;

public class InputManager : MonoBehaviour
{
  private static InputManager instance;

  [Header("Input Settings")]
  [SerializeField] private InputType _currentInputType = InputType.PC;
  [SerializeField] private bool _autoDetectPlatform = true;

  private bool isInitialized = false;

  public static InputManager Instance => instance;

  public InputType CurrentInputType => _currentInputType;
  public bool IsMobile => _currentInputType == InputType.Mobile;
  public bool IsPC => _currentInputType == InputType.PC;

  public Vector2 MoveInput { get; private set; }
  public Vector2 LookInput { get; private set; }
  public bool ShootPressed { get; private set; }
  public bool AimPressed { get; private set; }
  public bool JumpPressed { get; private set; }
  public bool DashPressed { get; private set; }

  private void Awake()
  {
    if (_autoDetectPlatform)
      DetectPlatform();

    if (instance != null && instance != this)
    {
      Destroy(this);
      return;
    }

    instance = GetComponent<InputManager>();
  }

  private void Start()
  {
    ApplyInputType();
    isInitialized = true;
  }

  private void DetectPlatform()
  {
    _currentInputType = InputType.PC;
    /*if (GP_Device.IsMobile())
      _currentInputType = InputType.Mobile;
    else
      _currentInputType = InputType.PC;*/
  }

  public void SetInputType(InputType inputType)
  {
    if (_currentInputType == inputType) return;

    _currentInputType = inputType;
    ApplyInputType();
  }

  private void ApplyInputType()
  {
    MoveInput = Vector2.zero;
    LookInput = Vector2.zero;
    ShootPressed = false;
    AimPressed = false;
    JumpPressed = false;
    DashPressed = false;
  }

  private void Update()
  {
    if (!isInitialized)
      return;

    UpdateInput();
  }

  private void UpdateInput()
  {
    switch (_currentInputType)
    {
      case InputType.PC:
        UpdatePCInput();
        break;
      case InputType.Mobile:
        UpdateMobileInput();
        break;
    }
  }

  private void UpdatePCInput()
  {
    MoveInput = Vector2.zero;
    LookInput = Vector2.zero;
    ShootPressed = false;
    AimPressed = false;
    JumpPressed = false;
    DashPressed = false;
  }

  private void UpdateMobileInput()
  {
    if (MoveInput.magnitude < 0.1f)
      MoveInput = Vector2.zero;

    if (LookInput.magnitude < 0.1f)
      LookInput = Vector2.zero;
    
    JumpPressed = false;
    DashPressed = false;
  }

  public void SetMoveInput(Vector2 parMoveInput)
  {
    MoveInput = parMoveInput;
  }

  public void SetLookInput(Vector2 parLookInput)
  {
    LookInput = parLookInput;
  }

  public bool GetKey(KeyCode key)
  {
    if (_currentInputType == InputType.PC)
    {
      return Input.GetKey(key);
    }
    else
    {
      switch (key)
      {
        case KeyCode.W: return MoveInput.y > 0.1f;
        case KeyCode.S: return MoveInput.y < -0.1f;
        case KeyCode.A: return MoveInput.x < -0.1f;
        case KeyCode.D: return MoveInput.x > 0.1f;
        case KeyCode.Space: return JumpPressed;
        default: return false;
      }
    }
  }

  public bool GetKeyDown(KeyCode key)
  {
    if (_currentInputType == InputType.PC)
    {
      return Input.GetKeyDown(key);
    }
    else
    {
      switch (key)
      {
        case KeyCode.Space: return JumpPressed;
        default: return false;
      }
    }
  }

  public bool GetMouseButton(int button)
  {
    if (_currentInputType == InputType.PC)
    {
      return Input.GetMouseButton(button);
    }
    else
    {
      switch (button)
      {
        case 0: return ShootPressed;
        case 1: return AimPressed;
        default: return false;
      }
    }
  }

  public bool GetMouseButtonDown(int button)
  {
    if (_currentInputType == InputType.PC)
    {
      return Input.GetMouseButtonDown(button);
    }
    else
    {
      switch (button)
      {
        case 0: return ShootPressed;
        case 1: return DashPressed;
        default: return false;
      }
    }
  }

  public float GetAxis(string axisName)
  {
    if (_currentInputType == InputType.PC)
    {
      return Input.GetAxis(axisName);
    }
    else
    {
      switch (axisName)
      {
        case "Mouse X": return LookInput.x;
        case "Mouse Y": return LookInput.y;
        case "Horizontal": return MoveInput.x;
        case "Vertical": return MoveInput.y;
        default: return 0f;
      }
    }
  }

  public void OnShootButtonDown()
  {
    if (_currentInputType == InputType.Mobile)
      ShootPressed = true;
  }

  public void OnShootButtonUp()
  {
    if (_currentInputType == InputType.Mobile)
      ShootPressed = false;
  }

  public void OnJumpButtonPressed()
  {
    if (_currentInputType == InputType.Mobile)
      JumpPressed = true;
  }

  public void OnDashButtonPressed()
  {
    if (_currentInputType == InputType.Mobile)
      DashPressed = true;
  }

  public void OnAimButtonPressed()
  {
    if (_currentInputType == InputType.Mobile)
      AimPressed = !AimPressed;
  }
}