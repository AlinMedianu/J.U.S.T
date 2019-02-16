using System;
using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Events;

namespace Com.SHUPDP.JUST
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        public static GameManager gameManager;
        public UnityEvent GameOver;

        int flooredTime;
        public float gametimer = 10.0f;
        public float respawnTimer = 3.0f;
        public GameObject TimeDisplay;
        public GameObject deathCamera;
        private Text Timetext;
        public GameObject playerKilledPopUp;

        public static GameManager Instance;
        [Tooltip("The prefab to use for representing the player")]

        public GameObject[] playerPrefabs;
        public Transform[] spawnPoints;
        public GameObject[] bloodDecals;

        private bool gamePlaying = true;

        void Awake()
        {
            if (gameManager == null)
            {
                gameManager = this;
            }
            else
            {
                Destroy(this);
            }
            // Move player to Launcher scene if not in room
            if (!PhotonNetwork.IsConnected)
                SceneManager.LoadScene(0);
        }

        void Start()
        {
            Instance = this;
            // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
            SpawnPlayer();
            Timetext = TimeDisplay.GetComponent<Text>();
            GameOver.AddListener(LoadEndScene);
            gamePlaying = true;
            // Set cursor to invisible for when xHair renders
            Cursor.visible = false;
            flooredTime = (int)gametimer;
        }


        #region Photon Callbacks

        /// <summary>
        /// Called when the local player left the room. We need to load the launcher/menu scene
        /// </summary>
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }

        public override void OnPlayerEnteredRoom(Player other)
        {
            Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName);

            //if (PhotonNetwork.IsMasterClient) // room reload not needed as room size will not change
            //{
            //    Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient);

            //    LoadArena();
            //}
        }

        public override void OnPlayerLeftRoom(Player other)
        {
            Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects

            //if (PhotonNetwork.IsMasterClient) // Room reload not needed as level size will not change.
            //{
            //    Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

            //    LoadArena();
            //}
        }

        #endregion

        #region Public Methods

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
            Cursor.visible = true;
        }

        #endregion

        private void Update()
        {
            if (gamePlaying)
            {
                int prevTimeDisp = flooredTime;
                gametimer -= Time.deltaTime;
                flooredTime = Mathf.FloorToInt(gametimer);
                if (gametimer <= 0)
                {
                    gametimer = 0;
                    GameOver.Invoke();
                    gamePlaying = false;
                }
                if (flooredTime != prevTimeDisp)
                {
                    DrawTime();
                }
            }
        }

        private void DrawTime()
        {
            Timetext.text = "TIME: " + flooredTime.ToString() + "S";
        }

        public void SpawnPlayer()
        {
            int spawnPointNum = UnityEngine.Random.Range(0, spawnPoints.Length);
            object materialColour;
            PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(JUSTConstantsAndDefinitions.customColour, out materialColour);
            if (materialColour == null)
            {
                Debug.Log("Randomised player colour");
                materialColour = UnityEngine.Random.Range(0, playerPrefabs.Length - 1);
            }
            GameObject newLocalPlayer = PhotonNetwork.Instantiate(this.playerPrefabs[(int)materialColour].name, spawnPoints[spawnPointNum].position, Quaternion.identity, 0);
            newLocalPlayer.GetComponent<Stats>().Name = PhotonNetwork.NickName;
            newLocalPlayer.name = PhotonNetwork.NickName;
            ((MonoBehaviour)newLocalPlayer.GetComponent<PlayerController>()).enabled = true; // Activate necessary components for player movement
        }

        public void Respawn(GameObject player)
        {
            StartCoroutine(RespawnPlayer(player));
        }

        public IEnumerator RespawnPlayer(GameObject player)
        {
            bool mine = player.GetComponent<PhotonView>().IsMine;
            Instantiate(bloodDecals[UnityEngine.Random.Range(0, bloodDecals.Length - 1)], (player.transform.position + new Vector3(0, 0.01f, 0)), player.transform.rotation);
            if (mine)
            {
                deathCamera.SetActive(true);
            }
            player.SetActive(false);
            yield return new WaitForSeconds(respawnTimer / 2);
            if (mine)
            {
            int spawnPointNum = UnityEngine.Random.Range(0, spawnPoints.Length);
            player.transform.position = spawnPoints[spawnPointNum].position;
            }
            yield return new WaitForSeconds(respawnTimer / 2);
            player.SetActive(true);
            player.GetComponent<PlayerInterface>().Respawn();
            if (mine)
            {
                deathCamera.SetActive(false);
            }
        }

        public void LoadEndScene()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel("EndGame");
            }
        }

        public void ShowKillPopup()
        {
            playerKilledPopUp.SetActive(true);
        }
    }
}
