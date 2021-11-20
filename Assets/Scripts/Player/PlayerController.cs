using UnityEngine;

/// <summary>
/// Provides a public static reference to the local player.
/// </summary>
public class PlayerController : MonoBehaviour
{
  public static PlayerController Local;

  public new Camera camera;
  public bool isLocal;

  // TODO Replace with some sort of inventory manager.
  public string currentlyEquippedItem;

  public void Start()
  {
    var fpController = GetComponent<FirstPersonController>();
    if (fpController != null && fpController.isLocalPlayer)
    {
      Local = this;
      this.isLocal = true;
    }
    GameManager.RegisterPlayer(this);
  }
}
