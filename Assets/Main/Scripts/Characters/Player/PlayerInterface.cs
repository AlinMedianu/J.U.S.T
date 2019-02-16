using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;

public class PlayerInterface : MonoBehaviourPunCallbacks {

    public Action Move = delegate { };
    public Action StartShooting = delegate { };
    public Action StopShooting = delegate { };
    public Action Reload = delegate { };
    public Action Aim = delegate { };
    public Action LowerAim = delegate { };
    public Action ToggleFlashlight = delegate { };
    public Action Interact = delegate { };
    public Action<int> HasBeenHit = delegate (int shooterId) { };
    //public Action HasPickedUpItem = delegate { };
    public Action HasBeenDetected = delegate { };
    public Action GetPoints = delegate { };
    public Action GetKill = delegate { };
    public Action Die = delegate { };
    public Action Respawn = delegate { };

    private void Awake()
    {
        if (photonView.IsMine)
        {
            (GetComponent<PlayerInputEvents>()).enabled = true;
        }
        
    }

    [PunRPC]
    public void RPCStartShooting()
    {
        StartShooting();
    }

    [PunRPC]
    public void RPCStopShooting()
    {
        StopShooting();
    }

    [PunRPC]
    public void RPCHasBeenHit(int shooterId)
    {
        HasBeenHit(shooterId);
    }


    [PunRPC]
    public void RPCToggleFlashlight()
    {
        ToggleFlashlight();
    }

    [PunRPC]
    public void RPCGetPoints()
    {
        GetPoints();
    }

    [PunRPC]
    public void RPCGetKill()
    {
        GetKill();
    }

    [PunRPC]
    public void RPCDie()
    {
        Die();
    }

    [PunRPC]
    public void RPCReload() //TODO Implement reloading animation etc
    {
        Reload();
    }
}
