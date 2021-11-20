using Mirror;
using UnityEngine;

public class TestPuzzleController : NetworkBehaviour
{
	public GameObject[] objectsToRecolour;
	public Material buttonOn, buttonOff;

	[SyncVar]
	private int buttonsEnabled = 0;

	public void OnButtonEnabled(AreaTrigger trigger)
	{
		buttonsEnabled++;
		UnityEngine.Debug.Log("TestPuzzleController.OnButtonEnabled() - client " + isClient + ", server " + isServer + ", authority " + hasAuthority + ", buttons held " + buttonsEnabled);
		SetOn(trigger, true, buttonsEnabled == 2);
	}

	public void OnButtonDisabled(AreaTrigger trigger)
	{
		buttonsEnabled--;
		UnityEngine.Debug.Log("TestPuzzleController.OnButtonDisabled() - client " + isClient + ", server " + isServer + ", authority " + hasAuthority + ", buttons held " + buttonsEnabled);
		SetOn(trigger, false, false);
	}

	[ClientRpc]
	private void SetOn(AreaTrigger trigger, bool on, bool lighthouseOn)
	{
		UnityEngine.Debug.Log("TestPuzzleController.SetOn() - client " + isClient + ", server " + isServer + ", authority " + hasAuthority + ", buttons held " + buttonsEnabled + ", on " + on + ", lighthouse on " + lighthouseOn);
		trigger.GetComponent<Renderer>().material = on ? buttonOn : buttonOff;
		if (lighthouseOn)
			foreach (GameObject obj in objectsToRecolour)
				obj.GetComponent<Renderer>().material = buttonOn;
	}
}
