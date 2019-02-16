using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


public class InGamePlayerPanel : MonoBehaviourPunCallbacks
{
    public GameObject playerUIEntryPrefab;

    // Use this for initialization
    void Start()
    {
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            GameObject entry = Instantiate(playerUIEntryPrefab);
            entry.transform.SetParent(this.transform);
            entry.transform.localScale = Vector3.one;
            entry.GetComponent<InGamePlayerDisplay>().Initialize(p.ActorNumber, p.NickName);
        }
    }

}