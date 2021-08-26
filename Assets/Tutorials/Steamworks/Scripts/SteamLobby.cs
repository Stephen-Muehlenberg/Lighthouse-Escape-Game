using Mirror;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;

namespace DapperDino.Mirror.Tutorials.Steamworks
{
    public class SteamLobby : MonoBehaviour
    {
        [SerializeField] private GameObject buttons = null;
    [SerializeField] private Text debugText;

        protected Callback<LobbyCreated_t> lobbyCreated;
        protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
        protected Callback<LobbyEnter_t> lobbyEntered;

        private const string HostAddressKey = "HostAddress";

        private NetworkManager networkManager;

        private void Start()
        {
      debugText.text += "\nSteamLobby.Start()";
            networkManager = GetComponent<NetworkManager>();

            if (!SteamManager.Initialized) { return; }

            lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
            gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
            lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        }

        public void HostLobby()
        {
      debugText.text += "\nSteamLobby.HostLobby()";
      buttons.SetActive(false);

            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, networkManager.maxConnections);
        }

        private void OnLobbyCreated(LobbyCreated_t callback)
        {
      debugText.text += "\nSteamLobby.OnLobbyCreated()";
      if (callback.m_eResult != EResult.k_EResultOK)
            {
                buttons.SetActive(true);
                return;
            }

            networkManager.StartHost();

            SteamMatchmaking.SetLobbyData(
                new CSteamID(callback.m_ulSteamIDLobby),
                HostAddressKey,
                SteamUser.GetSteamID().ToString());
        }

        private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
      debugText.text += "\nSteamLobby.OnGameLobbyJoinRequested()";

      SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
        }

        private void OnLobbyEntered(LobbyEnter_t callback)
    {
      debugText.text += "\nSteamLobby.OnLobbyEntered()";

      if (NetworkServer.active)
      {
        debugText.text += "\nJoined as host";
        return; 
      }
    else
        debugText.text += "\nJoined as client";

      string hostAddress = SteamMatchmaking.GetLobbyData(
                new CSteamID(callback.m_ulSteamIDLobby),
                HostAddressKey);
      debugText.text += "\nGot host address from lobby data: " + hostAddress;

      networkManager.networkAddress = hostAddress;

      debugText.text += "\nStarting client...";
      networkManager.StartClient();

            buttons.SetActive(false);

      debugText.text += "\nReady?";
    }
    }
}
