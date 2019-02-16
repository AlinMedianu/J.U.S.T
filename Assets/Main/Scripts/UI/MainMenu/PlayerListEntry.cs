using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Pun.UtilityScripts;

namespace Com.SHUPDP.JUST
{
    public class PlayerListEntry : MonoBehaviour
    {
        public Text PlayerNameText;

        public Image PlayerColorImage;
        public Button PlayerReadyButton;
        public Image PlayerReadyImage;

        private int ownerId;
        private bool isPlayerReady;

        #region Delegate Subscription/Removal

        // This can be used to change player colours/image icons in the lobby screen when players connect/disconnect
        // Photon.Pun.UtilityScripts.PlayerNumbering.OnPlayerNumberingChanged is the event handler called
        // when a players position in the room changes due to players leaving, etc.
        // By subscribing to it, we can run our own function when it happens, rather than checking every frame for changes
        public void OnEnable()
        {
            PlayerNumbering.OnPlayerNumberingChanged += OnPlayerNumberingChanged;
        }
        public void OnDisable()
        {
            PlayerNumbering.OnPlayerNumberingChanged -= OnPlayerNumberingChanged;
        }
        #endregion

        // Currently not using player colour icons, but may be added later
        private void OnPlayerNumberingChanged()
        {
            Debug.Log("OnPlayerNumberingChange called");
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                if (p.ActorNumber == ownerId)
                {
                    PlayerColorImage.color = JUSTConstantsAndDefinitions.GetColor(p.GetPlayerNumber());
                }
            }
        }

        public void Initialize(int playerId, string playerName)
        {
            ownerId = playerId;
            PlayerNameText.text = playerName;
        }

        public void Start()
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber != ownerId) // Check if the Prefab represents this player, otherwise hide the ready button
            {
                PlayerReadyButton.gameObject.SetActive(false);
            }
            else
            {
                Hashtable initialProps = new Hashtable { { JUSTConstantsAndDefinitions.readyStatus, isPlayerReady } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(initialProps);

                PlayerReadyButton.onClick.AddListener(() => // Add an event listener onto the ready button to update the properties
                {
                    isPlayerReady = !isPlayerReady;
                    SetPlayerReady(isPlayerReady);

                    Hashtable props = new Hashtable() { { JUSTConstantsAndDefinitions.readyStatus, isPlayerReady } };
                    PhotonNetwork.LocalPlayer.SetCustomProperties(props);

                    if (PhotonNetwork.IsMasterClient)
                    {
                        FindObjectOfType<MainMenuLobby>().LocalPlayerPropertiesUpdated();
                    }
                });
            }
        }
                public void SetPlayerReady(bool playerReady)
        {
            PlayerReadyButton.GetComponentInChildren<Text>().text = playerReady ? "READY!" : "READY?";
            PlayerReadyImage.enabled = playerReady;
        }
    }
}
