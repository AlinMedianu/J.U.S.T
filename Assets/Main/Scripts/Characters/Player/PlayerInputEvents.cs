using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerInputEvents : MonoBehaviour
{
    PhotonView photonView;
    PlayerInterface playerInterface;
    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        playerInterface = GetComponent<PlayerInterface>();
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                photonView.RPC("RPCStartShooting", RpcTarget.All);
            }
            else if (Input.GetButtonUp("Fire1"))
            {
                if (PhotonNetwork.InRoom)
                {
                    photonView.RPC("RPCStopShooting", RpcTarget.All);
                }
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                photonView.RPC("RPCReload", RpcTarget.All);
            }
            if (Input.GetKeyDown(KeyCode.F))
            {
                Debug.Log("torch");
                photonView.RPC("RPCToggleFlashlight", RpcTarget.All);
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                playerInterface.Interact();
            }
            if (Input.GetButtonDown("Fire2"))
            {
                playerInterface.Aim();
            }
            else if (Input.GetButtonUp("Fire2"))
            {
                if (PhotonNetwork.InRoom)
                {
                    playerInterface.LowerAim();
                }
            }
        }
    }
}
