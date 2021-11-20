using Mirror;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Object which can be looked at and worn by a player.
/// </summary>
public class Mask : NetworkBehaviour, LookTarget.InteractionManager
{
  public Text doorText;

  public bool CanInteract(PlayerController player)
  {
    return player.currentlyEquippedItem != "Mask";
  }

  public void Interact(PlayerController player, LookTarget _)
  {
    StartWearing(player);
    // TODO scramble code
  }

  public void StartWearing(PlayerController player)
  {
    var maskController = player.GetComponent<PlayerMaskController>();
    if (maskController.wearingMask) return;

    maskController.Wear(this);
    player.currentlyEquippedItem = "Mask";
  }
}
