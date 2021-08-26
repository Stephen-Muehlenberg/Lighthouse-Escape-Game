using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace DapperDino.Mirror.Tutorials.Steamworks
{
    public class MyNetworkManager : NetworkManager
  {
    [SerializeField] private Text debugText;
    [SerializeField] private string notificationMessage = string.Empty;

        public override void OnStartServer()
        {
      debugText.text += "\nMyNetworkManager.OnStartServer()";
      //         ServerChangeScene("Scene_SteamworksLobby");
      Camera.main.gameObject.SetActive(false);
        }

        [ContextMenu("Send Notification")]
        private void SendNotification()
        {
      debugText.text += "\nMyNetworkManager.SendNotification()";
      NetworkServer.SendToAll(new Notification { content = notificationMessage });
        }
    }
}
