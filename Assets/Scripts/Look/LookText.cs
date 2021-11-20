using UnityEngine;
using TMPro;

/// <summary>
/// Displays text when a player looks at the object.
/// </summary>
public class LookText : MonoBehaviour
{
  private static LookText Singleton;
  [SerializeField] private TextMeshProUGUI descriptionText;
  [SerializeField] private TextMeshProUGUI actionText;

  void Awake()
  {
    if (Singleton != null)
      throw new System.Exception("LookText is a singleton but multiple instances were found.");
    Singleton = this;
  }

  private void Start()
  {
    descriptionText.text = "";
    actionText.text = "";
  }

  public static void Show(string description, string action = null)
  {
    Singleton.descriptionText.text = description;
    Singleton.descriptionText.enabled = true;
    Singleton.actionText.text = 
      action == null ? ""
      : "(E) " + action;
    Singleton.actionText.enabled = action != null;
  }

  public static void Hide()
  {
    Singleton.descriptionText.enabled = false;
    Singleton.actionText.enabled = false;
  }
}
