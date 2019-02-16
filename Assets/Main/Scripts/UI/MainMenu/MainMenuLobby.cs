using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

using Hashtable = ExitGames.Client.Photon.Hashtable;
namespace Com.SHUPDP.JUST
{
    public class MainMenuLobby : MonoBehaviourPunCallbacks
    {
        public GameObject splashScreen;
        public GameObject mainMenuPanel;
        public GameObject multiplayerLogInPanel;
        public GameObject findAGamePanel;
        public GameObject createGamePanel;
        public GameObject joiningRandomGamePanel;
        public GameObject insideGameLobbyPanel;
        public Text gameLobbyTitle;
        public GameObject viewOpenGamesPanel;

        public InputField playerNameInputField;
        public InputField gameNameInputField;

        public Button StartGameButton;
        public UnityEngine.GameObject PlayerListEntryPrefab;

        public UnityEngine.GameObject RoomListPanel;

        public UnityEngine.GameObject RoomListContent;
        public UnityEngine.GameObject RoomListEntryPrefab;

        public Image BackgroundImageObject;
        public Sprite[] BackgroundImages;
        public AudioSource buttonClick;
        public AudioClip buttonClickSound;

        private Dictionary<string, RoomInfo> cachedRoomList;
        private Dictionary<string, UnityEngine.GameObject> roomListEntries;
        private Dictionary<int, UnityEngine.GameObject> playerListEntries;

        private void Awake()
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            cachedRoomList = new Dictionary<string, RoomInfo>();
            roomListEntries = new Dictionary<string, UnityEngine.GameObject>();
            string defaultName = string.Empty;

            if (PlayerPrefs.HasKey(JUSTConstantsAndDefinitions.playerNamePrefKey))
            {
                defaultName = PlayerPrefs.GetString(JUSTConstantsAndDefinitions.playerNamePrefKey);
                if (defaultName == "")
                {
                    defaultName = "Player " + Random.Range(1000, 10000);
                }
            }
            else
            {
                defaultName = "Player " + Random.Range(1000, 10000);
            }
            playerNameInputField.text = defaultName;
            PhotonNetwork.NickName = defaultName;
        }

        public void OnKeyPress()
        {
            this.SetActivePanel(mainMenuPanel.name);
            buttonClick.PlayOneShot(buttonClickSound);
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN");
            if (!PhotonNetwork.OfflineMode)
            {
                this.SetActivePanel(findAGamePanel.name);
            }
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            // TODO This is called when the online connection is lost. Could do with it going out to an error screen
            // For now its just kicking the player back to main menu
            Debug.Log("Disconnected from master");
            base.OnDisconnected(cause);
            this.SetActivePanel(mainMenuPanel.name);
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            SetActivePanel(findAGamePanel.name);
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            SetActivePanel(findAGamePanel.name);
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            string gameName = "Game " + Random.Range(1000, 10000);

            PhotonNetwork.CreateRoom(gameName, new RoomOptions { MaxPlayers = JUSTConstantsAndDefinitions.maxPlayersPerRoom });
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
            if (PhotonNetwork.OfflineMode)
            {
                // #Critical
                // Load the Room Level.
                PhotonNetwork.LoadLevel("Game");
            }
            else
            {
                SetActivePanel(insideGameLobbyPanel.name);

                if (playerListEntries == null)
                {
                    playerListEntries = new Dictionary<int, GameObject>();
                }

                foreach (Player p in PhotonNetwork.PlayerList)
                {
                    GameObject entry = Instantiate(PlayerListEntryPrefab);
                    entry.transform.SetParent(insideGameLobbyPanel.transform);
                    entry.transform.localScale = Vector3.one;
                    entry.GetComponent<PlayerListEntry>().Initialize(p.ActorNumber, p.NickName);

                    object isPlayerReady;
                    if (p.CustomProperties.TryGetValue(JUSTConstantsAndDefinitions.readyStatus, out isPlayerReady))
                    {
                        entry.GetComponent<PlayerListEntry>().SetPlayerReady((bool)isPlayerReady);
                    }

                    playerListEntries.Add(p.ActorNumber, entry);
                }

                StartGameButton.gameObject.SetActive(CheckPlayersReady());
                Hashtable networkedPlayerCustomProperties = new Hashtable
            {
                {JUSTConstantsAndDefinitions.readyStatus, false}
            };
                PhotonNetwork.LocalPlayer.SetCustomProperties(networkedPlayerCustomProperties);
            }
        }

        public override void OnLeftRoom()
        {
            SetActivePanel(findAGamePanel.name);

            foreach (GameObject entry in playerListEntries.Values)
            {
                Destroy(entry.gameObject);
            }

            playerListEntries.Clear();
            playerListEntries = null;
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            GameObject entry = Instantiate(PlayerListEntryPrefab);
            entry.transform.SetParent(insideGameLobbyPanel.transform);
            entry.transform.localScale = Vector3.one;
            entry.GetComponent<PlayerListEntry>().Initialize(newPlayer.ActorNumber, newPlayer.NickName);

            playerListEntries.Add(newPlayer.ActorNumber, entry);

            StartGameButton.gameObject.SetActive(CheckPlayersReady());
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Destroy(playerListEntries[otherPlayer.ActorNumber].gameObject);
            playerListEntries.Remove(otherPlayer.ActorNumber);

            StartGameButton.gameObject.SetActive(CheckPlayersReady());
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
            {
                StartGameButton.gameObject.SetActive(CheckPlayersReady());
            }
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (playerListEntries == null)
            {
                playerListEntries = new Dictionary<int, UnityEngine.GameObject>();
            }

            UnityEngine.GameObject entry;
            if (playerListEntries.TryGetValue(targetPlayer.ActorNumber, out entry))
            {
                object isPlayerReady;
                if (changedProps.TryGetValue(JUSTConstantsAndDefinitions.readyStatus, out isPlayerReady))
                {
                    entry.GetComponent<PlayerListEntry>().SetPlayerReady((bool)isPlayerReady);
                }
            }

            StartGameButton.gameObject.SetActive(CheckPlayersReady());
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            ClearRoomListView();

            UpdateCachedRoomList(roomList);
            UpdateRoomListView();
        }

        public override void OnLeftLobby()
        {
            cachedRoomList.Clear();

            ClearRoomListView();
        }

        private void SetActivePanel(string activePanel)
        {
            splashScreen.SetActive(activePanel.Equals(splashScreen.name));
            mainMenuPanel.SetActive(activePanel.Equals(mainMenuPanel.name));
            multiplayerLogInPanel.SetActive(activePanel.Equals(multiplayerLogInPanel.name));
            findAGamePanel.SetActive(activePanel.Equals(findAGamePanel.name));
            createGamePanel.SetActive(activePanel.Equals(createGamePanel.name));
            joiningRandomGamePanel.SetActive(activePanel.Equals(joiningRandomGamePanel.name));
            RoomListPanel.SetActive(activePanel.Equals(RoomListPanel.name));    // UI should call OnRoomListButtonClicked() to activate this
            insideGameLobbyPanel.SetActive(activePanel.Equals(insideGameLobbyPanel.name));
        }

        public void OnSinglePlayerButtonClicked()
        {
            if (!PhotonNetwork.InRoom)
            {
                PhotonNetwork.OfflineMode = true;
                PhotonNetwork.JoinRandomRoom();
            }
        }

        public void OnMultiplayerButtonClicked()
        {
            PhotonNetwork.OfflineMode = false;
            if (!PhotonNetwork.IsConnected)
            {
                this.SetActivePanel(multiplayerLogInPanel.name);
            }
            else
            {
                this.SetActivePanel(findAGamePanel.name);
            }
        }

        public void OnQuitButtonClicked()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public void OnSignInButtonClicked()
        {
            string playerName = playerNameInputField.text;

            if (!playerName.Equals(""))
            {
                PhotonNetwork.LocalPlayer.NickName = playerName;
                PhotonNetwork.ConnectUsingSettings();
                PlayerPrefs.SetString(JUSTConstantsAndDefinitions.playerNamePrefKey, playerName);
            }
            else
            {
                Debug.LogError("Player Name is invalid.");
            }
        }

        public void OnReturnToMainMenuButtonClicked()
        {
            SetActivePanel(mainMenuPanel.name);
        }

        public void OnJoinRandomRoomButtonClicked()
        {
            SetActivePanel(joiningRandomGamePanel.name);

            PhotonNetwork.JoinRandomRoom();
        }

        public void OnCreateGameButtonClicked()
        {
            SetActivePanel(createGamePanel.name);
            gameNameInputField.text = "Game " + Random.Range(1000, 10000);
        }

        public void OnLeaveLobbyCreationButtonClicked()
        {
            SetActivePanel(findAGamePanel.name);
        }

        public void OnLeaveMultiplayerButtonClicked()
        {
            PhotonNetwork.Disconnect();
            SetActivePanel(multiplayerLogInPanel.name);
        }

        public void OnCreateLobbyButtonClicked()
        {
            string gameName = gameNameInputField.text;
            gameName = (gameName.Equals(string.Empty)) ? "Game " + Random.Range(1000, 10000) : gameName;

            //gameLobbyTitle.text = "LOBBY - " + gameName;

            PhotonNetwork.CreateRoom(gameName, new RoomOptions { MaxPlayers = JUSTConstantsAndDefinitions.maxPlayersPerRoom });
        }
        public void OnLeaveLobbyButtonClicked()
        {
            PhotonNetwork.LeaveRoom();
        }

        public void OnStartGameButtonClicked()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                for (int i = 0; i < PhotonNetwork.PlayerList.Length; ++i)
                {
                    Hashtable networkedPlayerCustomColours = new Hashtable
            {
                {JUSTConstantsAndDefinitions.customColour, i}
            };
                    PhotonNetwork.PlayerList[i].SetCustomProperties(networkedPlayerCustomColours);
                }
            }
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;

            PhotonNetwork.LoadLevel("Game");
        }
        public void OnRoomListButtonClicked()
        {
            if (!PhotonNetwork.InLobby)
            {
                PhotonNetwork.JoinLobby();
            }

            SetActivePanel(RoomListPanel.name);
        }
        public void LocalPlayerPropertiesUpdated()
        {
            StartGameButton.gameObject.SetActive(CheckPlayersReady());
        }

        public void ButtonClicked()
        {
            buttonClick.PlayOneShot(buttonClickSound);
        }

        private bool CheckPlayersReady()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return false;
            }

            foreach (Player p in PhotonNetwork.PlayerList)
            {
                object isPlayerReady;
                if (p.CustomProperties.TryGetValue(JUSTConstantsAndDefinitions.readyStatus, out isPlayerReady))
                {
                    if (!(bool)isPlayerReady)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        private void UpdateCachedRoomList(List<RoomInfo> roomList)
        {
            foreach (RoomInfo info in roomList)
            {
                // Remove room from cached room list if it got closed, became invisible or was marked as removed
                if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
                {
                    if (cachedRoomList.ContainsKey(info.Name))
                    {
                        cachedRoomList.Remove(info.Name);
                    }

                    continue;
                }

                // Update cached room info
                if (cachedRoomList.ContainsKey(info.Name))
                {
                    cachedRoomList[info.Name] = info;
                }
                // Add new room info to cache
                else
                {
                    cachedRoomList.Add(info.Name, info);
                }
            }
        }

        private void UpdateRoomListView()
        {
            foreach (RoomInfo info in cachedRoomList.Values)
            {
                GameObject entry = Instantiate(RoomListEntryPrefab);
                entry.transform.SetParent(RoomListContent.transform);
                entry.transform.localScale = Vector3.one;
                entry.GetComponent<RoomListEntry>().Initialize(info.Name, (byte)info.PlayerCount, info.MaxPlayers);

                roomListEntries.Add(info.Name, entry);
            }
        }

        private void ClearRoomListView()
        {
            foreach (GameObject entry in roomListEntries.Values)
            {
                Destroy(entry.gameObject);
            }

            roomListEntries.Clear();
        }
    }
}
