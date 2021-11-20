using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// An object which displays popup text when looked at, and may optionally
/// be interactable.
/// </summary>
public class LookTarget : MonoBehaviour
{
  /// <summary>
  /// For dumb reasons, Func<>s can't be assigned in inspector,
  /// so to delegate the CanInteract() logic to another Behaviour
  /// it must implement this interface.
  /// </summary>
  public interface InteractionManager
  {
    public bool CanInteract(PlayerController player);
  }

  public string descriptionText;
  public string interactionText;
  public float interactionRange = 3;
  private InteractionManager interactionManager; // Workaround to delegate CanInteract().
  public UnityEvent<PlayerController, LookTarget> onStartLooking;
  public UnityEvent<PlayerController, LookTarget> onStopLooking;
  public UnityEvent<PlayerController, LookTarget> onInteract;

  public void Start()
  {
    interactionManager = GetComponent<InteractionManager>();
  }

  /// <summary>
  /// True if the specified <paramref name="player"/> can currently interact with this object.
  /// </summary>
  public bool CanInteract(PlayerController player)
    => interactionManager?.CanInteract(player) ?? true;

  /// <summary>
  /// Invoked when <paramref name="player"/> starts looking at this target while within
  /// <see cref="interactionRange"/>.
  /// </summary>
  public void StartLooking(PlayerController player)
  {
    LookText.Show(descriptionText, CanInteract(player) ? interactionText : string.Empty);
    onStartLooking?.Invoke(player, this);
  }

  /// <summary>
  /// Invoked when <paramref name="player"/> was previously looking at this target
  /// but stopped this frame (or exited the <see cref="interactionRange"/>).
  /// </summary>
  public void StopLooking(PlayerController player)
  {
    LookText.Hide();
    onStopLooking?.Invoke(player, this);
  }

  /// <summary>
  /// Invoked when <paramref name="player"/> clicks interact while looking at 
  /// this target (and within <see cref="interactionRange"/>).
  /// </summary>
  public void Interact(PlayerController player)
    => onInteract?.Invoke(player, this);
}
