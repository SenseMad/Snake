using FlamingCore;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerControl : MonoBehaviour
{
  [SerializeField]
  private float speed;

  [SerializeField]
  private float fastSpeed = 10f;

  [SerializeField]
  private float maxSpeed;

  [SerializeField]
  private float jumpSpeed = 2f;

  [SerializeField]
  private FpsCameraArm fpsCameraArm;

  public Transform sideCameraTransform;

  private Rigidbody rb;

  private Vector3 moveDirection;

  private Vector3 velocityTemp;

  private Transform pointer;

  private Combat combat;

  private float fpsPitch;

  public Transform groundChecker;

  public LayerMask groundLayer;

  private bool checkGround;

  public bool floatingMode;

  public bool dashMode;

  public float dashTime = 0.15f;

  private float dashTimer;

  public float dashCoolTime = 0.5f;

  private float dashCoolTimer;

  public float dashSpeed = 20f;

  private Vector3 dashDirection;

  public Transform firstPersonCameraTransform => fpsCameraArm.transform;

  public float MaxSpeed => maxSpeed;

  public Combat Combat => combat;

  private void Awake()
  {
	rb = GetComponent<Rigidbody>();
	combat = GetComponent<Combat>();
  }

  private void Start()
  {
	pointer = GameManager.Instance.LevelManager.Pointer;
  }

  private void Update()
  {
	if (!combat.IsDead && Time.timeScale != 0f)
	{
	  switch (GameManager.Instance.LevelManager.game3CType)
	  {
		case LevelManager.game3Ctypes.topDown:
		  UpdateTopDownMovement();
		  break;
		case LevelManager.game3Ctypes.fps:
		  UpdateFpsView();
		  UpdateFpsMovement();
		  break;
	  }
	}
  }

  private void FixedUpdate()
  {
	checkGround = CheckGround();
	if (!checkGround && GameManager.Instance.LevelManager.game3CType == LevelManager.game3Ctypes.fps)
	{
	  rb.AddForce(-Physics.gravity * 0.3f, ForceMode.Acceleration);
	}
  }

  private bool CheckGround()
  {
	Ray ray = default(Ray);
	ray.direction = Vector3.down;
	ray.origin = groundChecker.position;
	return Physics.Raycast(ray, 0.3f, groundLayer);
  }

  private void UpdateDash(Vector3 directionInput)
  {
	if (!dashMode)
	{
	  return;
	}
	if (dashCoolTimer > 0f)
	{
	  dashCoolTimer -= Time.deltaTime;
	}
	if (dashTimer > 0f)
	{
	  dashTimer -= Time.deltaTime;
	}

	bool dashInput = false;
	if (InputManager.Instance != null)
	{
      dashInput = InputManager.Instance.GetMouseButtonDown(1);
	}
	else
	{
	  dashInput = Input.GetMouseButtonDown(1);
	}

	if (dashInput && dashCoolTimer <= 0f)
	{
	  dashCoolTimer = dashCoolTime;
	  dashTimer = dashTime;
	  dashDirection = directionInput;
	  if (dashDirection.magnitude < 0.9f)
	  {
		dashDirection = base.transform.forward;
	  }
	  AudioManager.PlaySFXAtPosition("Dash", base.transform.position);
	}
	if (dashTimer > 0f)
	{
	  rb.velocity = dashDirection * dashSpeed;
	}
  }

  // Тестовая версия метода
  /*private void UpdateTopDownMovement()
  {
    Vector2 lookInput = Vector2.zero;
    Vector3 moveDir = Vector3.zero;

    if (Input.GetKey(KeyCode.A)) moveDir += Vector3.left;
    if (Input.GetKey(KeyCode.D)) moveDir += Vector3.right;
    if (Input.GetKey(KeyCode.W)) moveDir += Vector3.forward;
    if (Input.GetKey(KeyCode.S)) moveDir += Vector3.back;

    moveDir = moveDir.normalized;

    if (InputManager.Instance != null)
      lookInput = InputManager.Instance.LookInput;

    float currentSpeed = dashMode ? fastSpeed : speed;
    Vector3 camRot = Quaternion.Euler(0f, GameManager.Instance.CameraManager.TopDownCameraArm.transform.rotation.eulerAngles.y, 0f) * moveDir;
    velocityTemp = camRot.normalized * currentSpeed;
    velocityTemp.y = rb.velocity.y;
    rb.velocity = velocityTemp;

    bool rotated = false;

    if (lookInput.sqrMagnitude > 0.001f)
    {
      Vector3 lookDir = new Vector3(lookInput.x, 0f, lookInput.y);
      transform.rotation = Quaternion.LookRotation(lookDir, Vector3.up);
      rotated = true;
    }

    if (!rotated && moveDir.sqrMagnitude > 0.01f)
      transform.rotation = Quaternion.LookRotation(moveDir, Vector3.up);

    Vector3 forwardDir = FCTool.Vector3YToZero(transform.forward).normalized;
    UpdateDash(forwardDir);
  }*/

  private void UpdateTopDownMovement()
  {
    Vector2 moveInput = Vector2.zero;
    Vector2 lookInput = Vector2.zero;
    Vector3 moveDir = Vector3.zero;

    if (InputManager.Instance != null && InputManager.Instance.IsMobile)
    {
      moveInput = InputManager.Instance.MoveInput;
      lookInput = InputManager.Instance.LookInput;

      if (moveInput.magnitude > 0.1f)
        moveDir = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
    }
    else
    {
      if (Input.GetKey(KeyCode.A)) moveDir += Vector3.left;
      if (Input.GetKey(KeyCode.D)) moveDir += Vector3.right;
      if (Input.GetKey(KeyCode.W)) moveDir += Vector3.forward;
      if (Input.GetKey(KeyCode.S)) moveDir += Vector3.back;

      moveDir = moveDir.normalized;

      Vector3 pointerDir = FCTool.Vector3YToZero(pointer.position - transform.position);
      if (pointerDir.sqrMagnitude > 0.01f)
      {
        transform.rotation = Quaternion.LookRotation(pointerDir, Vector3.up);
      }
      else if (moveDir.sqrMagnitude > 0.01f)
      {
        transform.rotation = Quaternion.LookRotation(moveDir, Vector3.up);
      }
    }

    float currentSpeed = dashMode ? fastSpeed : speed;
    Vector3 camRot = Quaternion.Euler(0f, GameManager.Instance.CameraManager.TopDownCameraArm.transform.rotation.eulerAngles.y, 0f) * moveDir;
    velocityTemp = camRot.normalized * currentSpeed;
    velocityTemp.y = rb.velocity.y;
    rb.velocity = velocityTemp;

    if (InputManager.Instance != null && InputManager.Instance.IsMobile)
    {
      bool rotated = false;

      /*if (lookInput.sqrMagnitude > 0.001f)
      {
        Vector3 lookDir = new Vector3(lookInput.x, 0f, lookInput.y);
        transform.rotation = Quaternion.LookRotation(lookDir, Vector3.up);
        rotated = true;
      }*/
      if (lookInput.sqrMagnitude > 0.001f)
      {
        Vector3 localLook = new Vector3(lookInput.x, 0f, lookInput.y);

        Quaternion camYRot = Quaternion.Euler(0f, GameManager.Instance.CameraManager.TopDownCameraArm.transform.eulerAngles.y, 0f);
        Vector3 worldLookDir = camYRot * localLook;

        transform.rotation = Quaternion.LookRotation(worldLookDir.normalized, Vector3.up);
        rotated = true;
      }

      if (!rotated && moveDir.sqrMagnitude > 0.01f)
        transform.rotation = Quaternion.LookRotation(moveDir, Vector3.up);
    }

    Vector3 forwardDir = FCTool.Vector3YToZero(transform.forward).normalized;
    UpdateDash(forwardDir);
  }

  private void UpdateFpsMovement()
  {
	Vector3 a = Vector3.zero;

	if (InputManager.Instance != null && InputManager.Instance.IsMobile)
	{
	  Vector2 moveInput = InputManager.Instance.MoveInput;
	  if (moveInput.magnitude > 0.1f)
	  {
		a = new Vector3(moveInput.x, 0f, moveInput.y);
	  }
	}
	else
	{
	  if (UnityEngine.Input.GetKey(KeyCode.A))
	  {
		a += Vector3.left;
	  }
	  if (UnityEngine.Input.GetKey(KeyCode.D))
	  {
		a += Vector3.right;
	  }
	  if (UnityEngine.Input.GetKey(KeyCode.W))
	  {
		a += Vector3.forward;
	  }
	  if (UnityEngine.Input.GetKey(KeyCode.S))
	  {
		a += Vector3.back;
	  }
	}

	a = a.normalized;
	float num = speed;
	if (dashMode)
	{
	  num = fastSpeed;
	}
	if ((bool)GameManager.Instance.CameraManager.FpsCameraArm)
	{
	  if (GameManager.Instance.CameraManager.FpsCameraArm.Ads)
	  {
		num *= 0.7f;
	  }
	  velocityTemp = (FCTool.Vector3YToZero(GameManager.Instance.CameraManager.FpsCameraArm.transform.forward).normalized * a.z + FCTool.Vector3YToZero(GameManager.Instance.CameraManager.FpsCameraArm.transform.right).normalized * a.x).normalized * num;
	  velocityTemp.y = rb.velocity.y;
	}

	bool jumpInput = false;
	if (InputManager.Instance != null && InputManager.Instance.IsMobile)
	{
	  jumpInput = InputManager.Instance.GetKeyDown(KeyCode.Space);
	}
	else
	{
	  jumpInput = UnityEngine.Input.GetKeyDown(KeyCode.Space);
	}

	if (jumpInput && checkGround)
	{
	  velocityTemp.y = jumpSpeed;
	}

	bool spaceHeld = false;
	if (InputManager.Instance != null && InputManager.Instance.IsMobile)
	{
	  spaceHeld = InputManager.Instance.GetKey(KeyCode.Space);
	}
	else
	{
	  spaceHeld = UnityEngine.Input.GetKey(KeyCode.Space);
	}

	if (spaceHeld && floatingMode && velocityTemp.y < 0f)
	{
	  velocityTemp.y = 0f;
	}
	rb.velocity = velocityTemp;
	UpdateDash((FCTool.Vector3YToZero(GameManager.Instance.CameraManager.FpsCameraArm.transform.forward).normalized * a.z + FCTool.Vector3YToZero(GameManager.Instance.CameraManager.FpsCameraArm.transform.right).normalized * a.x).normalized);
  }

  private void UpdateFpsView()
  {
	float mouseX, mouseY;

	if (InputManager.Instance != null && InputManager.Instance.IsMobile)
	{
	  mouseX = InputManager.Instance.GetAxis("Mouse X");
	  mouseY = InputManager.Instance.GetAxis("Mouse Y");
	}
	else
	{
	  mouseX = UnityEngine.Input.GetAxis("mouse x");
	  mouseY = UnityEngine.Input.GetAxis("mouse y");
	}

	base.transform.rotation = Quaternion.Euler(0f, base.transform.rotation.eulerAngles.y + mouseX * fpsCameraArm.Sensitivity, 0f);
	fpsPitch += mouseY * (0f - fpsCameraArm.Sensitivity);
	fpsPitch = Mathf.Clamp(fpsPitch, -89.9f, 89.9f);
	firstPersonCameraTransform.localRotation = Quaternion.Euler(fpsPitch, 0f, 0f);
  }

  private Vector3 Vector3YToZero(Vector3 v3)
  {
	return new Vector3(v3.x, 0f, v3.z);
  }
}