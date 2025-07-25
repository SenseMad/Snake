using FlamingCore;
using System.Collections.Generic;
using UnityEngine;

public class Aimer : MonoBehaviour
{
  public enum aimWays
  {
	aimPointer,
	aimMoveDirection,
	aimPlayer
  }

  public Combat combat;

  public aimWays aimWay;

  public Rigidbody rigidbody;

  [Space]
  [Header("Joints")]
  [Space]
  public Transform needAim;

  public Transform clone;

  private bool gameStarted;

  private Transform pointer;

  private PlayerControl player;

  private Vector3 lookVector;

  [SerializeField]
  private bool playerAimer;

  public List<Aimer> subAimers;

  private bool isSubAimer;

  [SerializeField]
  private bool aimZonFpsMode;

  [SerializeField] private bool _isPlayer;

  public void SetAsSubAimer()
  {
	isSubAimer = true;
  }

  private void Start()
  {
	for (int i = 0; i < subAimers.Count; i++)
	{
	  if ((bool)subAimers[i])
	  {
		subAimers[i].SetAsSubAimer();
	  }
	}
  }

  private void OnDestroy()
  {
    player.Combat.OnRevive -= Combat_OnRevive;
  }

  private void Update()
  {
	if (!isSubAimer)
	{
	  UpdateAim();
	}
	for (int i = 0; i < subAimers.Count; i++)
	{
	  if ((bool)subAimers[i])
	  {
		subAimers[i].UpdateAim();
	  }
	}
  }
  
  public void UpdateAim2()
  {
    if (!gameStarted)
    {
      if (GameManager.Instance.LevelManager.GameState == LevelManager.gameStates.playing)
      {
        gameStarted = true;
        player = GameManager.Instance.LevelManager.Player;
        pointer = GameManager.Instance.LevelManager.Pointer;
      }
    }
    else
    {
      if (GameManager.Instance.LevelManager.game3CType == LevelManager.game3Ctypes.topDown && _isPlayer)
      {
        if (InputManager.Instance != null && InputManager.Instance.IsMobile && aimWay == aimWays.aimPointer && pointer != null)
        {
          float fixedDistance = 5f;
          pointer.position = player.transform.position + player.transform.forward * fixedDistance;
        }

        return;
      }

      if ((playerAimer && GameManager.Instance.LevelManager.game3CType == LevelManager.game3Ctypes.fps) || combat.IsDead)
        return;

      if (aimWay == aimWays.aimPlayer)
      {
        base.transform.rotation = Quaternion.LookRotation(FCTool.Vector3YToZero(GameManager.Instance.LevelManager.Player.transform.position - base.transform.position), Vector3.up);
        if (aimZonFpsMode && GameManager.Instance.LevelManager.game3CType == LevelManager.game3Ctypes.fps)
        {
          base.transform.rotation = Quaternion.LookRotation((GameManager.Instance.LevelManager.Player.transform.position - base.transform.position).normalized, Vector3.up);
        }
      }
      else
      {
        if (!(combat.Part != null) || !combat.Part.Actived)
          return;

        if (aimWay == aimWays.aimPointer)
        {
          if (!player.Combat.IsDead)
          {
            base.transform.rotation = Quaternion.LookRotation(FCTool.Vector3YToZero(pointer.position - base.transform.position), Vector3.up);
          }
        }
        else if (aimWay == aimWays.aimMoveDirection && rigidbody != null)
        {
          lookVector = FCTool.Vector3YToZero(rigidbody.velocity);
          if (lookVector.magnitude > 0.15f)
          {
            base.transform.rotation = Quaternion.LookRotation(lookVector, Vector3.up);
          }
        }
      }
    }
  }

  public void UpdateAim()
  {
	if (!gameStarted)
	{
	  if (GameManager.Instance.LevelManager.GameState == LevelManager.gameStates.playing)
	  {
		gameStarted = true;
		player = GameManager.Instance.LevelManager.Player;
		pointer = GameManager.Instance.LevelManager.Pointer;

        player.Combat.OnRevive += Combat_OnRevive;
      }
	}
	else
	{
      if (GameManager.Instance.LevelManager.game3CType == LevelManager.game3Ctypes.topDown && _isPlayer)
      {
        if (InputManager.Instance != null && InputManager.Instance.IsMobile && aimWay == aimWays.aimPointer && pointer != null)
        {
          float fixedDistance = 5f;
          pointer.position = player.transform.position + player.transform.forward * fixedDistance;
        }

        return;
      }

      if ((playerAimer && GameManager.Instance.LevelManager.game3CType == LevelManager.game3Ctypes.fps) || combat.IsDead)
        return;

      if (aimWay == aimWays.aimPlayer)
	  {
		base.transform.rotation = Quaternion.LookRotation(FCTool.Vector3YToZero(GameManager.Instance.LevelManager.Player.transform.position - base.transform.position), Vector3.up);
		if (aimZonFpsMode && GameManager.Instance.LevelManager.game3CType == LevelManager.game3Ctypes.fps)
		{
		  base.transform.rotation = Quaternion.LookRotation((GameManager.Instance.LevelManager.Player.transform.position - base.transform.position).normalized, Vector3.up);
		}
	  }
	  else
	  {
		if (!(combat.Part != null) || !combat.Part.Actived)
          return;

        bool isMobile = InputManager.Instance != null && InputManager.Instance.IsMobile;

        if (aimWay == aimWays.aimPointer)
        {
          if (isMobile)
          {
            float fixedDistance = 5f;
            pointer.position = player.transform.position + player.transform.forward * fixedDistance;

            transform.rotation = Quaternion.LookRotation(FCTool.Vector3YToZero(pointer.position - transform.position), Vector3.up);
          }
          else
          {
            transform.rotation = Quaternion.LookRotation(FCTool.Vector3YToZero(pointer.position - transform.position), Vector3.up);
          }
          if (!player.Combat.IsDead && isMobile)
          {
            transform.rotation = Quaternion.LookRotation(FCTool.Vector3YToZero(pointer.position - transform.position), Vector3.up);
          }
        }
        else if (aimWay == aimWays.aimMoveDirection && rigidbody != null)
        {
          lookVector = FCTool.Vector3YToZero(rigidbody.velocity);
          if (lookVector.magnitude > 0.15f)
          {
            transform.rotation = Quaternion.LookRotation(lookVector, Vector3.up);
          }
        }
        /*if (aimWay == aimWays.aimPointer)
		{
		  if (!player.Combat.IsDead)
		  {
			transform.rotation = Quaternion.LookRotation(FCTool.Vector3YToZero(pointer.position - transform.position), Vector3.up);
		  }
		}
        else if (aimWay == aimWays.aimMoveDirection && rigidbody != null)
		{
		  lookVector = FCTool.Vector3YToZero(rigidbody.velocity);
		  if (lookVector.magnitude > 0.15f)
		  {
			base.transform.rotation = Quaternion.LookRotation(lookVector, Vector3.up);
		  }
		}*/
      }
	}
  }

  private void LateUpdate()
  {
	if (!isSubAimer)
	{
	  LateUpdateAim();
	}
	for (int i = 0; i < subAimers.Count; i++)
	{
	  if ((bool)subAimers[i])
	  {
		subAimers[i].LateUpdateAim();
	  }
	}
  }

  public void LateUpdateAim()
  {
	if (gameStarted && needAim != null && clone != null)
	{
	  needAim.transform.rotation = clone.transform.rotation;
	}
  }

  private void Combat_OnRevive()
  {
    if (aimWay == aimWays.aimMoveDirection && rigidbody != null)
    {
      lookVector = FCTool.Vector3YToZero(rigidbody.velocity);
      if (lookVector.magnitude > 0.15f)
        transform.rotation = Quaternion.LookRotation(lookVector, Vector3.up);
    }
  }
}