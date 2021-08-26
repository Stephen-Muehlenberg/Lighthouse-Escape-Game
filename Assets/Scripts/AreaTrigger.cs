using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class AreaTrigger : NetworkBehaviour
{
  public UnityEvent<AreaTrigger> onTriggerEnabled;
  public UnityEvent<AreaTrigger> onTriggerDisabled;
  public UnityEvent<AreaTrigger, GameObject> onPlayerEntered;
  public UnityEvent<AreaTrigger, GameObject> onPlayerExited;

  public int playersInArea { get; private set; }
  public bool triggered { get; private set; }

  [ServerCallback]
  void OnTriggerEnter(Collider co)
  {
    UnityEngine.Debug.Log("ArearTrigger.OnTriggerEnter()");
    if (!isServer) return;
    UnityEngine.Debug.Log("- AAA - player tag : " + co.tag);

    if (co.CompareTag("Player"))
    {
      UnityEngine.Debug.Log("on enetered - authority ? " + hasAuthority + ", server ? " + isServer);
      playersInArea++;
      onPlayerEntered?.Invoke(this, co.gameObject);
    }
  }

  [ServerCallback]
  void OnTriggerExit(Collider co)
  {
    if (!isServer) return;

    if (co.CompareTag("Player"))
    {
      UnityEngine.Debug.Log("on exited - authority ? " + hasAuthority + ", server ? " + isServer);
      playersInArea--;
      onPlayerExited?.Invoke(this, co.gameObject);
    }
  }

  void Update()
  {
    if (!isServer) return;

    if (playersInArea > 0)
    {
      if (!triggered)
      {
        onTriggerEnabled?.Invoke(this);
        triggered = true;
      }
    } else
    {
      if (triggered)
      {
        onTriggerDisabled?.Invoke(this);
        triggered = false;
      }
    }
  }
}
