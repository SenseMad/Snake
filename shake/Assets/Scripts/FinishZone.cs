using System.Collections.Generic;
using UnityEngine;
using GamePush;

public class FinishZone : MonoBehaviour
{
  public float radius;

  protected bool canSuccess;

  public GameObject finshObj;

  public GameObject unfinshObj;

  protected bool locked;

  [Space]
  [Header("selecter")]
  [Space]
  public bool isSlecter;

  public bool forceUnlock;

  public int selectIndex = 1;

  public List<Renderer> renderers;

  public Material lockMaterial;

  public TextMesh text;

  public Color lockedTextColor;

  public LevelManager.gameModes gameMode;

  public LevelManager.game3Ctypes game3CType;

  protected virtual void Start()
  {
	if (!isSlecter)
	{
	  finshObj.SetActive(value: false);
	  unfinshObj.SetActive(value: true);
	}
	else if (!DataManager.EverEnteredLevel("Level" + selectIndex.ToString()) && !forceUnlock)
	{
	  locked = true;
	  text.color = lockedTextColor;
	  for (int i = 0; i < renderers.Count; i++)
	  {
		renderers[i].material = lockMaterial;
	  }
	}
  }

  protected virtual void Update()
  {
	if (!isSlecter)
	{
	  if (!canSuccess && GameManager.Instance.LevelManager.CheckCanSuccess())
	  {
		canSuccess = true;
		finshObj.SetActive(value: true);
		unfinshObj.SetActive(value: false);
	  }
	  else if (canSuccess && !GameManager.Instance.LevelManager.CheckCanSuccess())
	  {
		canSuccess = false;
		finshObj.SetActive(value: false);
		unfinshObj.SetActive(value: true);
	  }
	  if (canSuccess && Vector3.Distance(GameManager.Instance.LevelManager.Player.transform.position, base.transform.position) < radius)
	  {
		GameManager.Instance.LevelManager.Success();
	  }
	}
	else if (!locked && Vector3.Distance(GameManager.Instance.LevelManager.Player.transform.position, base.transform.position) < radius)
	{
	  GameManager.Instance.LevelManager.gameMode = gameMode;
	  GameManager.Instance.LevelManager.game3CType = game3CType;
	  GameManager.Instance.LevelManager.TryLoadLevel(selectIndex);
	}
  }

  private void OnDrawGizmos()
  {
	Gizmos.color = Color.white;
	Gizmos.DrawWireSphere(base.transform.position, radius);
  }
}