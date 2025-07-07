using UnityEngine;

public abstract class DropItemBase : MonoBehaviour, IDropItem
{
  public abstract void OnPickup(GameObject parPicker);

  protected virtual void OnTriggerEnter(Collider other)
  {
    if (!other.GetComponent<PlayerControl>())
      return;

    OnPickup(gameObject);
  }
}