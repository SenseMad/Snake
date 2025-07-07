using Com.LuisPedroFonseca.ProCamera2D;
using FlamingCore;
using UnityEngine;

public class TopDownCameraArm : MonoBehaviour
{
  [Space]
  [Header("CameraBehaviour")]
  [Space]
  public Transform pointerReceiver;

  [Space]
  [Header("Death")]
  [Space]
  public float deadFOV = 30f;
  public float deadDistance = 10f;
  public float deadFadeInTime;
  public AnimationCurve deadCurve;

  private Camera virtualCamera;
  private ProCamera2D proCamera2d;
  private ProCamera2DShake shakeComponent;

  private bool dead;
  private float deadFadeInTimer;
  private float deadFadeLerp;

  private float startFov;
  private float startDistance;
  private Quaternion startRotation;

  private Transform target;
  private bool gameStarted;
  private bool waitOneFrame;

  private float startHeight;

  private Quaternion normalRotation;
  private float normalFov;
  private Vector3 normalPosition;
  private bool cameraInitialized = false;

  public Camera VirtualCamera => virtualCamera;
  public ProCamera2D ProCamera2d => proCamera2d;

  private void Awake()
  {
    Init();
  }

  private void Init()
  {
    virtualCamera = GetComponent<Camera>();
    proCamera2d = GetComponent<ProCamera2D>();
    shakeComponent = GetComponent<ProCamera2DShake>();
  }

  private void Update()
  {
    if (!waitOneFrame)
    {
      waitOneFrame = true;
    }
    else if (!gameStarted)
    {
      if (GameManager.Instance.LevelManager.GameState == LevelManager.gameStates.playing)
      {
        gameStarted = true;
        target = GameManager.Instance.LevelManager.Player.transform;

        if (!cameraInitialized)
        {
          normalFov = virtualCamera.fieldOfView;
          normalPosition = virtualCamera.transform.position;
          normalRotation = virtualCamera.transform.rotation;
          cameraInitialized = true;
        }

        startHeight = virtualCamera.transform.position.y - target.position.y;
      }
    }
    else
    {
      UpdateHeight();
      UpdatePointerReceiver();
    }
  }

  public void LateUpdate()
  {
    if (gameStarted && dead)
    {
      if (deadFadeInTimer < deadFadeInTime)
      {
        deadFadeInTimer += Time.deltaTime;
        deadFadeInTimer = Mathf.Clamp(deadFadeInTimer, 0f, deadFadeInTime);
        deadFadeLerp = deadCurve.Evaluate(deadFadeInTimer / deadFadeInTime);
      }
      UpdateDeadBehaviour();
    }
  }

  public void UpdateHeight()
  {
    if (!dead && target != null)
    {
      transform.position = Vector3.Lerp(
          transform.position,
          FCTool.Vector3YToZero(transform.position) + Vector3.up * (target.position.y + startHeight),
          8f * Time.deltaTime
      );
    }
  }

  private void UpdatePointerReceiver()
  {
    if (target != null && pointerReceiver != null)
    {
      pointerReceiver.transform.position = Vector3.Lerp(pointerReceiver.transform.position, target.position, 6f * Time.unscaledDeltaTime);
    }
  }

  public void Over()
  {
    dead = true;
    proCamera2d.enabled = false;
    shakeComponent.enabled = false;

    startFov = virtualCamera.fieldOfView;
    startDistance = Vector3.Distance(virtualCamera.transform.position, target.position);
    startRotation = virtualCamera.transform.rotation;
  }

  public void RestoreCameraState(Transform newTarget)
  {
    dead = false;
    deadFadeInTimer = 0f;
    deadFadeLerp = 0f;

    proCamera2d.enabled = true;
    shakeComponent.enabled = true;

    target = newTarget;

    proCamera2d.MoveCameraInstantlyToPosition(new Vector2(normalPosition.x, normalPosition.z));

    transform.rotation = normalRotation;
    virtualCamera.fieldOfView = normalFov;

    startHeight = normalPosition.y - newTarget.position.y;

    if (pointerReceiver != null)
      pointerReceiver.position = newTarget.position;
  }

  private void UpdateDeadBehaviour()
  {
    virtualCamera.fieldOfView = Mathf.Lerp(startFov, deadFOV, deadFadeLerp);
    virtualCamera.transform.position = FCTool.Vector3YToZero(virtualCamera.transform.position) +
                                       Vector3.up * Mathf.Lerp(virtualCamera.transform.position.y, target.position.y + 5.5f, deadFadeLerp);
    virtualCamera.transform.rotation = Quaternion.Lerp(startRotation, Quaternion.LookRotation(target.position - virtualCamera.transform.position), deadFadeLerp);
  }
}