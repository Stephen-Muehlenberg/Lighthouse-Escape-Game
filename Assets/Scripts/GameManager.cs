using UnityEngine;

public class GameManager : MonoBehaviour
{
  private static GameManager Singleton;

  public GameObject[] componentsToEnable; // When normal gameplay starts
  public GameObject[] componentsToDisable; // When normal gameplay starts
  public LookController lookController;

  public void Awake()
  {
    if (Singleton != null) throw new System.Exception("GameManager is a singleton but multiple instances were created.");
    Singleton = this;
  }

  /// <summary>
  /// Initializes normal gameplay mode.
  /// </summary>
  public static void EnableNormalGameplay()
  {
    if (Singleton.componentsToEnable != null && Singleton.componentsToEnable.Length > 0)
      foreach (GameObject o in Singleton.componentsToEnable)
        o.SetActive(true);
    if (Singleton.componentsToDisable != null && Singleton.componentsToDisable.Length > 0)
      foreach (GameObject o in Singleton.componentsToDisable)
        o.SetActive(false);
  }

  public static void RegisterPlayer(PlayerController player)
  {
    UnityEngine.Debug.Log("GameManager.RegisterPlayer()");
    if (PlayerController.Local != null)
    {
      UnityEngine.Debug.Log("- binding look controller...");
      Singleton.lookController.BindTo(PlayerController.Local);
      Singleton.lookController.enabled = true;
      UnityEngine.Debug.Log("- look controller enabled ? " + Singleton.lookController.enabled);
    }
  }
}
