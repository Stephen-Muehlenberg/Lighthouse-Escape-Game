using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Checks if the player is looking at an interactable object, displaying
/// popup text and handling interactions if so.
/// </summary>
public class LookController : MonoBehaviour
{
  private const float MAX_LOOK_RANGE = 100f;

  [SerializeField] private InputAction interactAction;

  private PlayerController player;
  private Transform viewOrigin;
  private int layerMask;
  private LookTarget currentTarget;
  private LookTarget previousTarget;

  // Cached for reuse every frame.
  private bool rayHitSomething;
  private RaycastHit hitInfo;

  void Start()
  {
    layerMask = ~LayerMask.GetMask("Invisible Wall", "Player", "UI", "Ignore Raycast"); // Ignore these layers
  }

  public void BindTo(PlayerController player)
  {
    UnityEngine.Debug.Log("LookController.BindTo()");
    this.player = player;
    viewOrigin = player.camera.transform;
    interactAction.Enable();
    interactAction.performed += Interact;
  }

  void Update()
  {
//    if (!active) return;
  //  if (TimeUtils.gameplayPaused) return;

    rayHitSomething = Physics.Raycast(viewOrigin.position, viewOrigin.forward, out hitInfo, MAX_LOOK_RANGE, layerMask);

    // Not looking at anything.
    if (!rayHitSomething)
    {
      StopLookingAtPreviousTarget();
      return;
    }

    // No LookTargets being looked at.
    currentTarget = hitInfo.collider.GetComponent<LookTarget>();
    if (currentTarget == null || !currentTarget.enabled)
    {
      StopLookingAtPreviousTarget();
      return;
    }

    // Target is too far to interact with.
    if (hitInfo.distance > currentTarget.interactionRange)
    {
      StopLookingAtPreviousTarget();
      return;
    }

    if (previousTarget != null)
    {
      if (currentTarget == previousTarget)
      {
        // Can only interact with a target if you didnt start looking at it this frame
//        if (Input.GetButtonUp("PrimaryFire")) currentTarget.interact();
        // TODO Handle interaction
        return;
      }
      else StopLookingAtPreviousTarget();
    }

    UnityEngine.Debug.Log("LookController. start looking at " + currentTarget.name);
    currentTarget.StartLooking(player);
    previousTarget = currentTarget;
  }

  private void StopLookingAtPreviousTarget()
  {
    if (previousTarget == null) return;

    previousTarget.StopLooking(player);
    previousTarget = null;
  }

  private void Interact(InputAction.CallbackContext context)
  {
    if (currentTarget != null && currentTarget.CanInteract(player))
      currentTarget.Interact(player);
  }
}
