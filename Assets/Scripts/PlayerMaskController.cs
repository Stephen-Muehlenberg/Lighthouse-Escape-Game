using UnityEngine;

/// <summary>
/// Manages mask interactions / status for a single player.
/// </summary>
/// TODO This class might be worth making abstract and reusing for other player interaction managers.
public class PlayerMaskController : MonoBehaviour
{
  public Mask equippedMask { get; private set; }
  public bool wearingMask => equippedMask != null;

  public bool Wear(Mask mask)
  {
    if (wearingMask) return false;
    equippedMask = mask;
    mask.transform.SetParent(this.GetComponent<PlayerController>().camera.transform);
    mask.transform.localPosition = new Vector3(0, 0, 0.35f);
    mask.transform.localRotation = Quaternion.identity;
    return true;
  }
}
