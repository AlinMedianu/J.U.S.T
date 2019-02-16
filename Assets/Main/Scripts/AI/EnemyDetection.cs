using UnityEngine;
using Photon.Pun;
using System;

[RequireComponent(typeof(PhotonView))]
public class EnemyDetection : MonoBehaviour
{
    private bool[] playerDetectionStates;
    private int noPlayers = 0;
    [SerializeField]
    private float visionRange = 30f;                         
    private GameObject[] players;
    private PhotonView photonView;

    public event Action OnPlayerDetected = () => { };
    public event Action OnPlayerLost = () => { };

    public Transform Target { get; set; }          //the first player to be seen by this enemy 

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        photonView.RPC("GetAllPlayers", RpcTarget.AllBuffered);
    }

    private void Update()
    {
        for (int playerID = 0; playerID < noPlayers; ++playerID)
            if (playerDetectionStates[playerID])
            {
                if(IsPlayerDetectable(players[playerID]))
                {
                    Target = players[playerID].transform;
                    photonView.RPC("PlayerDetected", RpcTarget.All);
                    playerDetectionStates[playerID] = false;
                }
            }
            else if (!IsPlayerDetectable(players[playerID]))
            {
                photonView.RPC("PlayerLost", RpcTarget.All);
                playerDetectionStates[playerID] = true;
            }
    }

    private bool IsPlayerDetectable(GameObject player)
    {
        Vector3 currentPosition = transform.position;
        Vector3 playerposition = player.transform.position;
        ++playerposition.y;
        RaycastHit visionBlock;
        if (Physics.Raycast(currentPosition, playerposition - currentPosition, out visionBlock, visionRange, ~(1 << 11 | 1 << 12)))
            return visionBlock.collider.gameObject == player;
        return false;
    }

    [PunRPC]
    private void GetAllPlayers()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        noPlayers = players.Length;
        playerDetectionStates = new bool[noPlayers];
        for (int playerID = 0; playerID < noPlayers; ++playerID)
            playerDetectionStates[playerID] = true;
    }

    [PunRPC]
    private void PlayerDetected()
    {
        OnPlayerDetected();
    }

    [PunRPC]
    private void PlayerLost()
    {
        OnPlayerLost();
    }
}
